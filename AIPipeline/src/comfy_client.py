"""
ComfyUI stylization stage of the AI sprite pipeline (Phase 6.3).

Takes the per-frame "beauty" (matte-gray, transparent bg) and "depth"
(ControlNet-Depth convention) renders produced by blender_render.py and
turns each depth frame into a stylized dark-fantasy sprite frame via a
real ComfyUI SDXL + ControlNet-Depth txt2img workflow
(workflows/dark_fantasy_sprite.json). The beauty frame's own alpha
channel (the render already has a transparent background) is then used
as a cutout mask against the stylized output -- since the pose was
locked by ControlNet-Depth, the silhouettes line up and no separate
background-removal model (e.g. BiRefNet) is required.

This is a REAL implementation: it talks to a live ComfyUI HTTP API
(upload/image, /prompt, /history, /view). There is no mock fallback --
any server error, missing node, or degenerate (blank/near-uniform)
output raises loudly.

Typical usage (server already running on 127.0.0.1:8188):

    AIPipeline/.venv/bin/python AIPipeline/src/comfy_client.py \\
        --input AIPipeline/temp/render_momask_out \\
        --output AIPipeline/temp/stylized_out \\
        --frames 0,5,6,10 --seed 12345

If the server is not running, this script will try to start it itself
(subprocess against ComfyUI's own venv) unless --no-auto-start is
passed, in which case it fails with a clear message telling you how to
start it manually.

Phase 7.1 -- "Consistency": default mode is now a two-pass, IPAdapter-
conditioned generation so all frames of a clip depict the same
character (same armor/palette), instead of each frame being an
independent SDXL sample that only shares its pose (via ControlNet-
Depth). Pass 1 renders one "hero" frame (mid-action frame by default)
with the plain single-pass workflow (workflows/dark_fantasy_sprite.json,
no IPAdapter). Pass 2 regenerates every requested frame -- hero frame
included -- with workflows/dark_fantasy_sprite_ipadapter.json, which
adds an IPAdapter (SDXL vit-h) branch conditioned on the pass-1 hero
image on top of the existing per-frame ControlNet-Depth conditioning.
Pass `--no-ipadapter` to fall back to the old single-pass behavior, or
`--reference <img>` to skip pass 1 and condition on an explicit
reference image instead (this is also the hook for a future fixed
character-design image).
"""
import argparse
import copy
import json
import os
import socket
import subprocess
import sys
import time
import urllib.parse
import uuid
from urllib.parse import urlparse

try:
    import requests
except ImportError:  # pragma: no cover - clearer error than a bare traceback
    print(
        "[ComfyUI] Missing dependency 'requests'. Install it into this "
        "interpreter's venv, e.g.:\n"
        "    AIPipeline/.venv/bin/python -m pip install requests Pillow numpy",
        file=sys.stderr,
    )
    raise

try:
    from PIL import Image, ImageFilter
except ImportError:  # pragma: no cover
    print(
        "[ComfyUI] Missing dependency 'Pillow'. Install it into this "
        "interpreter's venv, e.g.:\n"
        "    AIPipeline/.venv/bin/python -m pip install requests Pillow numpy",
        file=sys.stderr,
    )
    raise

try:
    import numpy as np
except ImportError:  # pragma: no cover
    np = None


# ---------------------------------------------------------------------------
# Defaults / node-id map for workflows/dark_fantasy_sprite.json
# ---------------------------------------------------------------------------

_HERE = os.path.dirname(os.path.abspath(__file__))
DEFAULT_WORKFLOW_PATH = os.path.join(_HERE, "..", "workflows", "dark_fantasy_sprite.json")
DEFAULT_IPADAPTER_WORKFLOW_PATH = os.path.join(
    _HERE, "..", "workflows", "dark_fantasy_sprite_ipadapter.json"
)
DEFAULT_COMFYUI_URL = "http://127.0.0.1:8188"
# NAOR_AI_TOOLS_DIR is the single env var that gates where the whole external
# AI toolchain (ComfyUI + MoMask, ~12GB, not in version control) lives -- see
# AIPipeline/setup_ai_tools.sh. COMFYUI_DIR is kept as a legacy override for
# anyone who still has it set, but NAOR_AI_TOOLS_DIR takes precedence.
NAOR_AI_TOOLS_DIR = os.environ.get(
    "NAOR_AI_TOOLS_DIR", "/Volumes/aebasol_1tb/Ob/Projects/game_NAOR/AI_Tools"
)
DEFAULT_COMFYUI_DIR = os.environ.get(
    "COMFYUI_DIR", os.path.join(NAOR_AI_TOOLS_DIR, "ComfyUI")
)

# Node ids inside dark_fantasy_sprite.json (API format). Keep in sync with
# the workflow JSON -- these are the nodes this client patches per frame.
NODE_POSITIVE = "6"
NODE_NEGATIVE = "7"
NODE_LATENT = "5"
NODE_CONTROL_IMAGE = "11"
NODE_CONTROL_APPLY = "12"
NODE_KSAMPLER = "3"
NODE_SAVE = "9"

# Extra node ids present only in dark_fantasy_sprite_ipadapter.json (API
# format) -- the IPAdapter reference-image branch. Absent from the plain
# single-pass workflow, which is why build_workflow() checks for their
# presence before patching them.
NODE_IPA_REF_IMAGE = "22"
NODE_IPA_APPLY = "23"

DEFAULT_IPADAPTER_WEIGHT = 0.8

POSITIVE_TEMPLATE = (
    "dark fantasy game character, {subject}, 2d game art, high detail, "
    "dramatic rim lighting, solid dark background"
)
DEFAULT_SUBJECT = "gothic knight swinging a heavy sword, blasphemous style"
DEFAULT_NEGATIVE = (
    "photo, photorealistic, 3d render, cgi, blurry, blur, low detail, "
    "extra limbs, missing limbs, deformed hands, deformed face, watermark, "
    "text, signature, jpeg artifacts, oversaturated, flat lighting, "
    "plain background, multiple characters, cropped"
)


class ComfyUIError(RuntimeError):
    """Raised on any ComfyUI server / generation failure. Never swallowed."""


# ---------------------------------------------------------------------------
# Server lifecycle
# ---------------------------------------------------------------------------

def _parse_host_port(base_url):
    parsed = urlparse(base_url)
    return parsed.hostname or "127.0.0.1", parsed.port or 8188


def is_server_up(base_url, timeout=3.0):
    try:
        resp = requests.get(f"{base_url}/system_stats", timeout=timeout)
        return resp.status_code == 200
    except requests.exceptions.RequestException:
        return False


def wait_for_server(base_url, timeout=120.0, poll_interval=1.0, log_path=None):
    start = time.time()
    while time.time() - start < timeout:
        if is_server_up(base_url):
            return True
        time.sleep(poll_interval)
    tail = ""
    if log_path and os.path.exists(log_path):
        with open(log_path, "r", errors="replace") as f:
            lines = f.readlines()
        tail = "".join(lines[-40:])
    raise ComfyUIError(
        f"ComfyUI did not come up at {base_url} within {timeout}s.\n"
        f"--- tail of {log_path} ---\n{tail}"
    )


def start_server(comfyui_dir, comfyui_python, log_path):
    if not os.path.isdir(comfyui_dir):
        raise ComfyUIError(f"ComfyUI directory not found: {comfyui_dir}")
    if not os.path.exists(comfyui_python):
        raise ComfyUIError(f"ComfyUI python interpreter not found: {comfyui_python}")
    log_f = open(log_path, "w")
    proc = subprocess.Popen(
        [comfyui_python, "main.py"],
        cwd=comfyui_dir,
        stdout=log_f,
        stderr=subprocess.STDOUT,
    )
    return proc, log_f


def stop_server(proc, log_f=None):
    if proc is None:
        return
    print("[ComfyUI] Stopping server we started...")
    proc.terminate()
    try:
        proc.wait(timeout=20)
    except subprocess.TimeoutExpired:
        proc.kill()
        proc.wait(timeout=10)
    if log_f is not None:
        try:
            log_f.close()
        except Exception:
            pass


def ensure_server(base_url, auto_start, comfyui_dir, comfyui_python, start_timeout, log_path):
    """Returns (proc, log_f) if we started the server (caller must stop it
    when done), or (None, None) if it was already running."""
    if is_server_up(base_url):
        print(f"[ComfyUI] Server already running at {base_url}")
        return None, None

    if not auto_start:
        raise ComfyUIError(
            f"ComfyUI server not reachable at {base_url} and --no-auto-start "
            f"was passed. Start it manually:\n"
            f"    cd {comfyui_dir} && ./venv/bin/python main.py"
        )

    print(f"[ComfyUI] Server not running at {base_url}; starting it "
          f"({comfyui_python} main.py in {comfyui_dir})...")
    proc, log_f = start_server(comfyui_dir, comfyui_python, log_path)
    wait_for_server(base_url, timeout=start_timeout, log_path=log_path)
    print(f"[ComfyUI] Server is up at {base_url}")
    return proc, log_f


# ---------------------------------------------------------------------------
# HTTP API helpers
# ---------------------------------------------------------------------------

def upload_image(base_url, filepath, subfolder="naor_stylize", image_type="input"):
    with open(filepath, "rb") as f:
        files = {"image": (os.path.basename(filepath), f, "image/png")}
        data = {"type": image_type, "subfolder": subfolder, "overwrite": "true"}
        resp = requests.post(f"{base_url}/upload/image", files=files, data=data, timeout=30)
    if resp.status_code != 200:
        raise ComfyUIError(f"/upload/image failed ({resp.status_code}): {resp.text}")
    payload = resp.json()
    return payload["name"], payload.get("subfolder", subfolder)


def queue_prompt(base_url, workflow, client_id):
    resp = requests.post(
        f"{base_url}/prompt",
        json={"prompt": workflow, "client_id": client_id},
        timeout=30,
    )
    if resp.status_code != 200:
        try:
            detail = resp.json()
        except ValueError:
            detail = resp.text
        raise ComfyUIError(f"/prompt rejected the workflow ({resp.status_code}): {detail}")
    payload = resp.json()
    if "prompt_id" not in payload:
        raise ComfyUIError(f"/prompt response missing prompt_id: {payload}")
    return payload["prompt_id"]


def wait_for_completion(base_url, prompt_id, poll_interval=2.0, timeout=600.0):
    start = time.time()
    while True:
        if time.time() - start > timeout:
            raise ComfyUIError(
                f"Timed out after {timeout}s waiting for prompt {prompt_id} to finish."
            )
        resp = requests.get(f"{base_url}/history/{prompt_id}", timeout=30)
        if resp.status_code != 200:
            raise ComfyUIError(f"/history/{prompt_id} failed ({resp.status_code}): {resp.text}")
        data = resp.json()
        entry = data.get(prompt_id)
        if entry is not None:
            status = entry.get("status", {}) or {}
            status_str = status.get("status_str")
            if status_str == "error":
                raise ComfyUIError(
                    f"ComfyUI reported an error for prompt {prompt_id}: "
                    f"{json.dumps(status.get('messages', []), default=str)}"
                )
            if status.get("completed") or entry.get("outputs"):
                return entry
        time.sleep(poll_interval)


def fetch_output_image_bytes(base_url, entry, save_node_id):
    outputs = entry.get("outputs", {})
    node_out = outputs.get(save_node_id)
    if not node_out or not node_out.get("images"):
        raise ComfyUIError(
            f"History entry has no images for SaveImage node {save_node_id}: {outputs}"
        )
    img_ref = node_out["images"][0]
    params = {
        "filename": img_ref["filename"],
        "subfolder": img_ref.get("subfolder", ""),
        "type": img_ref.get("type", "output"),
    }
    resp = requests.get(f"{base_url}/view", params=params, timeout=60)
    if resp.status_code != 200:
        raise ComfyUIError(f"/view failed ({resp.status_code}) for {img_ref}: {resp.text}")
    return resp.content


# ---------------------------------------------------------------------------
# Workflow patching
# ---------------------------------------------------------------------------

def load_workflow(path):
    with open(path, "r") as f:
        return json.load(f)


def build_workflow(
    template,
    control_image_name,
    positive_text,
    negative_text,
    seed,
    steps,
    cfg,
    sampler_name,
    scheduler,
    controlnet_strength,
    width,
    height,
    filename_prefix,
    reference_image_name=None,
    ipadapter_weight=None,
):
    wf = copy.deepcopy(template)

    def node(node_id):
        if node_id not in wf:
            raise ComfyUIError(
                f"Workflow is missing expected node id '{node_id}' -- did "
                f"dark_fantasy_sprite.json change shape?"
            )
        return wf[node_id]

    node(NODE_POSITIVE)["inputs"]["text"] = positive_text
    node(NODE_NEGATIVE)["inputs"]["text"] = negative_text
    node(NODE_CONTROL_IMAGE)["inputs"]["image"] = control_image_name
    node(NODE_CONTROL_APPLY)["inputs"]["strength"] = controlnet_strength
    node(NODE_LATENT)["inputs"]["width"] = width
    node(NODE_LATENT)["inputs"]["height"] = height
    ks = node(NODE_KSAMPLER)["inputs"]
    ks["seed"] = seed
    ks["steps"] = steps
    ks["cfg"] = cfg
    ks["sampler_name"] = sampler_name
    ks["scheduler"] = scheduler
    node(NODE_SAVE)["inputs"]["filename_prefix"] = filename_prefix

    # IPAdapter branch -- only present in dark_fantasy_sprite_ipadapter.json.
    if NODE_IPA_REF_IMAGE in wf:
        if reference_image_name is None:
            raise ComfyUIError(
                f"Workflow has IPAdapter node '{NODE_IPA_REF_IMAGE}' but no "
                f"reference_image_name was supplied to build_workflow()."
            )
        node(NODE_IPA_REF_IMAGE)["inputs"]["image"] = reference_image_name
        if ipadapter_weight is not None:
            node(NODE_IPA_APPLY)["inputs"]["weight"] = ipadapter_weight
    elif reference_image_name is not None:
        raise ComfyUIError(
            "reference_image_name was supplied but this workflow template "
            "has no IPAdapter node -- pass the IPAdapter workflow path or "
            "drop the reference image."
        )
    return wf


# ---------------------------------------------------------------------------
# Sanity check + alpha cutout
# ---------------------------------------------------------------------------

def assert_image_not_degenerate(pil_img, label, std_threshold=4.0):
    if np is None:
        return  # numpy not available -- skip the statistical check
    arr = np.asarray(pil_img.convert("L"), dtype=np.float32)
    std = float(arr.std())
    if std < std_threshold:
        raise ComfyUIError(
            f"Generated image '{label}' looks degenerate (grayscale std="
            f"{std:.2f} < {std_threshold}); this usually means the sampler "
            f"produced a blank/black frame or pure noise. Refusing to treat "
            f"this as a successful stylization."
        )


def apply_alpha_cutout(raw_img, beauty_path, dilate_px=4):
    """Cut the stylized SDXL output down to the beauty-render silhouette.

    The beauty frame is a transparent-background render of the SAME pose
    (the depth ControlNet locked the SDXL generation to that pose), so its
    alpha channel is a usable cutout mask for the stylized frame. The mask
    is dilated a few pixels first so the hard body silhouette doesn't chew
    into e.g. hair/cloth/weapon edges the model painted slightly outside
    the exact body contour.
    """
    raw = raw_img.convert("RGBA")
    beauty = Image.open(beauty_path).convert("RGBA")
    if beauty.size != raw.size:
        beauty = beauty.resize(raw.size, Image.LANCZOS)
    mask = beauty.split()[-1]  # alpha channel, mode 'L'
    if dilate_px > 0:
        kernel = dilate_px * 2 + 1
        mask = mask.filter(ImageFilter.MaxFilter(kernel))
    out = raw.copy()
    out.putalpha(mask)
    return out


# ---------------------------------------------------------------------------
# Frame discovery
# ---------------------------------------------------------------------------

def discover_frames(input_dir, frame_indices=None):
    depth_files = sorted(f for f in os.listdir(input_dir) if f.startswith("depth_") and f.endswith(".png"))
    if not depth_files:
        raise ComfyUIError(f"No depth_*.png frames found in {input_dir}")

    frames = []
    for fname in depth_files:
        idx_str = fname[len("depth_"):-len(".png")]
        try:
            idx = int(idx_str)
        except ValueError:
            continue
        beauty_name = f"frame_{idx_str}.png"
        beauty_path = os.path.join(input_dir, beauty_name)
        if not os.path.exists(beauty_path):
            raise ComfyUIError(
                f"Depth frame {fname} has no matching beauty frame {beauty_name} in {input_dir}"
            )
        if frame_indices is not None and idx not in frame_indices:
            continue
        frames.append((idx, os.path.join(input_dir, fname), beauty_path))

    if frame_indices is not None:
        missing = set(frame_indices) - {f[0] for f in frames}
        if missing:
            raise ComfyUIError(f"Requested frame indices not found in {input_dir}: {sorted(missing)}")

    frames.sort(key=lambda t: t[0])
    return frames


# ---------------------------------------------------------------------------
# Main entry point
# ---------------------------------------------------------------------------

def _generate_frame(
    comfyui_url,
    client_id,
    workflow_template,
    control_image_name,
    positive_prompt,
    negative_prompt,
    seed,
    steps,
    cfg,
    sampler_name,
    scheduler,
    controlnet_strength,
    width,
    height,
    filename_prefix,
    poll_interval,
    poll_timeout,
    label,
    reference_image_name=None,
    ipadapter_weight=None,
):
    """Builds + queues one workflow and returns the raw PIL image (RGB, not
    yet cutout). Shared by the hero pass and the per-frame pass(es)."""
    workflow = build_workflow(
        workflow_template,
        control_image_name=control_image_name,
        positive_text=positive_prompt,
        negative_text=negative_prompt,
        seed=seed,
        steps=steps,
        cfg=cfg,
        sampler_name=sampler_name,
        scheduler=scheduler,
        controlnet_strength=controlnet_strength,
        width=width,
        height=height,
        filename_prefix=filename_prefix,
        reference_image_name=reference_image_name,
        ipadapter_weight=ipadapter_weight,
    )

    prompt_id = queue_prompt(comfyui_url, workflow, client_id)
    print(f"[ComfyUI] {label}: queued as prompt_id={prompt_id}, waiting...")
    entry = wait_for_completion(comfyui_url, prompt_id, poll_interval=poll_interval, timeout=poll_timeout)
    img_bytes = fetch_output_image_bytes(comfyui_url, entry, NODE_SAVE)

    from io import BytesIO
    raw_img = Image.open(BytesIO(img_bytes))
    raw_img.load()
    assert_image_not_degenerate(raw_img, label=label)
    return raw_img, img_bytes


def stylize_frames(
    input_dir,
    output_dir,
    prompt_theme=DEFAULT_SUBJECT,
    negative_prompt=DEFAULT_NEGATIVE,
    seed=12345,
    steps=30,
    cfg=7.0,
    sampler_name="dpmpp_2m",
    scheduler="karras",
    controlnet_strength=0.9,
    width=1024,
    height=1024,
    frame_indices=None,
    comfyui_url=DEFAULT_COMFYUI_URL,
    auto_start=True,
    comfyui_dir=DEFAULT_COMFYUI_DIR,
    comfyui_python=None,
    start_timeout=120.0,
    poll_timeout=600.0,
    poll_interval=2.0,
    workflow_path=DEFAULT_WORKFLOW_PATH,
    mask_dilate_px=4,
    use_ipadapter=True,
    ipadapter_workflow_path=DEFAULT_IPADAPTER_WORKFLOW_PATH,
    ipadapter_weight=DEFAULT_IPADAPTER_WEIGHT,
    reference_image_path=None,
    hero_frame_index=None,
):
    """Stylizes depth_%04d.png frames from input_dir into transparent-bg
    dark-fantasy sprite frames in output_dir/frame_%04d.png, keeping the
    pre-cutout raw SDXL output in output_dir/raw/frame_%04d.png.

    Two modes:

    - use_ipadapter=False: original Milestone-6 single-pass behavior --
      every requested frame is generated independently from
      `workflow_path` (SDXL + ControlNet-Depth only). Poses match (via
      ControlNet) but armor/palette can flicker frame to frame.

    - use_ipadapter=True (default, Phase 7.1): two-pass "consistent"
      mode. Pass 1 renders one hero frame with `workflow_path` (no
      IPAdapter) -- either the frame at `reference_image_path` (if an
      explicit external reference image is given, pass 1 is skipped
      entirely) or the frame at `hero_frame_index` (default: the middle
      frame of the requested set). Pass 2 regenerates every requested
      frame (hero included) with `ipadapter_workflow_path`, which adds
      an IPAdapter branch conditioned on the pass-1 hero image on top
      of the same per-frame ControlNet-Depth conditioning, so all
      frames share the hero's design/palette.

    Raises ComfyUIError (or lets requests/IOError propagate) on any
    failure -- there is no silent-mock fallback.
    """
    if comfyui_python is None:
        comfyui_python = os.path.join(comfyui_dir, "venv", "bin", "python")

    os.makedirs(output_dir, exist_ok=True)
    raw_dir = os.path.join(output_dir, "raw")
    os.makedirs(raw_dir, exist_ok=True)

    frames = discover_frames(input_dir, frame_indices)
    print(f"[ComfyUI] {len(frames)} frame(s) queued for stylization from {input_dir}")

    workflow_template = load_workflow(workflow_path)
    positive_prompt = POSITIVE_TEMPLATE.format(subject=prompt_theme)
    print(f"[ComfyUI] Positive prompt: {positive_prompt}")
    print(f"[ComfyUI] Negative prompt: {negative_prompt}")
    print(f"[ComfyUI] seed={seed} steps={steps} cfg={cfg} sampler={sampler_name}/{scheduler} "
          f"controlnet_strength={controlnet_strength} size={width}x{height} "
          f"use_ipadapter={use_ipadapter}")

    log_path = os.path.join(output_dir, "comfyui_server.log")
    proc, log_f = ensure_server(comfyui_url, auto_start, comfyui_dir, comfyui_python, start_timeout, log_path)
    client_id = str(uuid.uuid4())

    total_start = time.time()
    per_frame_seconds = []
    hero_seconds = None
    try:
        reference_server_name = None
        ipadapter_workflow_template = None

        if use_ipadapter:
            ipadapter_workflow_template = load_workflow(ipadapter_workflow_path)

            if reference_image_path:
                print(f"[ComfyUI] --- reference image (explicit, pass 1 skipped) ---")
                print(f"[ComfyUI] Using external reference: {reference_image_path}")
                ref_name, ref_subfolder = upload_image(comfyui_url, reference_image_path)
                reference_server_name = ref_name if not ref_subfolder else f"{ref_subfolder}/{ref_name}"
            else:
                if hero_frame_index is not None:
                    hero_tuple = next((f for f in frames if f[0] == hero_frame_index), None)
                    if hero_tuple is None:
                        raise ComfyUIError(
                            f"--hero-frame {hero_frame_index} is not among the requested "
                            f"frames {[f[0] for f in frames]}"
                        )
                else:
                    hero_tuple = frames[len(frames) // 2]
                hero_idx, hero_depth_path, hero_beauty_path = hero_tuple
                print(f"[ComfyUI] --- pass 1: hero frame {hero_idx:04d} (no IPAdapter) ---")

                hero_start = time.time()
                hero_server_name, hero_subfolder = upload_image(comfyui_url, hero_depth_path)
                hero_control_name = (
                    hero_server_name if not hero_subfolder else f"{hero_subfolder}/{hero_server_name}"
                )
                hero_img, hero_bytes = _generate_frame(
                    comfyui_url, client_id, workflow_template,
                    control_image_name=hero_control_name,
                    positive_prompt=positive_prompt,
                    negative_prompt=negative_prompt,
                    seed=seed, steps=steps, cfg=cfg,
                    sampler_name=sampler_name, scheduler=scheduler,
                    controlnet_strength=controlnet_strength,
                    width=width, height=height,
                    filename_prefix=f"dark_fantasy_sprite_hero_{hero_idx:04d}",
                    poll_interval=poll_interval, poll_timeout=poll_timeout,
                    label=f"hero frame {hero_idx:04d}",
                )
                hero_dir = os.path.join(output_dir, "hero")
                os.makedirs(hero_dir, exist_ok=True)
                hero_raw_path = os.path.join(hero_dir, f"raw_hero_{hero_idx:04d}.png")
                with open(hero_raw_path, "wb") as f:
                    f.write(hero_bytes)
                hero_cutout = apply_alpha_cutout(hero_img, hero_beauty_path, dilate_px=mask_dilate_px)
                hero_cutout_path = os.path.join(hero_dir, f"hero_{hero_idx:04d}.png")
                hero_cutout.save(hero_cutout_path)
                hero_seconds = time.time() - hero_start
                print(f"[ComfyUI] hero frame {hero_idx:04d} done in {hero_seconds:.1f}s "
                      f"-> {hero_raw_path} (reference for pass 2)")

                # Re-upload the raw (pre-cutout) hero image as the IPAdapter
                # reference for pass 2 -- IPAdapter conditions on the visual
                # content/style of the reference image, and the prompt
                # already asks for a solid dark background so the raw
                # (non-transparent) output works fine as-is.
                ref_name, ref_subfolder = upload_image(comfyui_url, hero_raw_path)
                reference_server_name = ref_name if not ref_subfolder else f"{ref_subfolder}/{ref_name}"

        for idx, depth_path, beauty_path in frames:
            frame_start = time.time()
            pass_label = "pass 2 (IPAdapter)" if use_ipadapter else "single-pass"
            print(f"[ComfyUI] --- frame {idx:04d} ({pass_label}) ---")

            server_name, server_subfolder = upload_image(comfyui_url, depth_path)
            control_image_name = server_name if not server_subfolder else f"{server_subfolder}/{server_name}"

            if use_ipadapter:
                raw_img, img_bytes = _generate_frame(
                    comfyui_url, client_id, ipadapter_workflow_template,
                    control_image_name=control_image_name,
                    positive_prompt=positive_prompt,
                    negative_prompt=negative_prompt,
                    seed=seed, steps=steps, cfg=cfg,
                    sampler_name=sampler_name, scheduler=scheduler,
                    controlnet_strength=controlnet_strength,
                    width=width, height=height,
                    filename_prefix=f"dark_fantasy_sprite_ipa_{idx:04d}",
                    poll_interval=poll_interval, poll_timeout=poll_timeout,
                    label=f"frame {idx:04d}",
                    reference_image_name=reference_server_name,
                    ipadapter_weight=ipadapter_weight,
                )
            else:
                raw_img, img_bytes = _generate_frame(
                    comfyui_url, client_id, workflow_template,
                    control_image_name=control_image_name,
                    positive_prompt=positive_prompt,
                    negative_prompt=negative_prompt,
                    seed=seed, steps=steps, cfg=cfg,
                    sampler_name=sampler_name, scheduler=scheduler,
                    controlnet_strength=controlnet_strength,
                    width=width, height=height,
                    filename_prefix=f"dark_fantasy_sprite_{idx:04d}",
                    poll_interval=poll_interval, poll_timeout=poll_timeout,
                    label=f"frame {idx:04d}",
                )

            raw_out_path = os.path.join(raw_dir, f"frame_{idx:04d}.png")
            with open(raw_out_path, "wb") as f:
                f.write(img_bytes)

            final_img = apply_alpha_cutout(raw_img, beauty_path, dilate_px=mask_dilate_px)
            final_out_path = os.path.join(output_dir, f"frame_{idx:04d}.png")
            final_img.save(final_out_path)

            elapsed = time.time() - frame_start
            per_frame_seconds.append(elapsed)
            print(f"[ComfyUI] frame {idx:04d} done in {elapsed:.1f}s -> {final_out_path}")
    finally:
        stop_server(proc, log_f)

    total_elapsed = time.time() - total_start
    if per_frame_seconds:
        avg = sum(per_frame_seconds) / len(per_frame_seconds)
        hero_note = f" (+ {hero_seconds:.1f}s hero pass)" if hero_seconds is not None else ""
        print(
            f"[ComfyUI] Stylization complete: {len(per_frame_seconds)} frame(s), "
            f"{total_elapsed:.1f}s total{hero_note}, {avg:.1f}s/frame average."
        )
    return per_frame_seconds


def _parse_frames_arg(value):
    if value is None or value.strip().lower() == "all":
        return None
    indices = []
    for chunk in value.split(","):
        chunk = chunk.strip()
        if not chunk:
            continue
        indices.append(int(chunk))
    return indices


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--input", required=True, help="Dir with frame_%%04d.png + depth_%%04d.png")
    parser.add_argument("--output", required=True, help="Dir to write stylized frame_%%04d.png (+ raw/)")
    parser.add_argument("--prompt-theme", default=DEFAULT_SUBJECT,
                         help="Character/action clause slotted into the dark-fantasy prompt template")
    parser.add_argument("--negative-prompt", default=DEFAULT_NEGATIVE)
    parser.add_argument("--seed", type=int, default=12345)
    parser.add_argument("--steps", type=int, default=30)
    parser.add_argument("--cfg", type=float, default=7.0)
    parser.add_argument("--sampler", default="dpmpp_2m")
    parser.add_argument("--scheduler", default="karras")
    parser.add_argument("--controlnet-strength", type=float, default=0.9)
    parser.add_argument("--width", type=int, default=1024)
    parser.add_argument("--height", type=int, default=1024)
    parser.add_argument("--frames", default="all", help="'all' or comma-separated frame indices, e.g. 0,5,6,10")
    parser.add_argument("--comfyui-url", default=DEFAULT_COMFYUI_URL)
    parser.add_argument("--comfyui-dir", default=DEFAULT_COMFYUI_DIR)
    parser.add_argument("--comfyui-python", default=None)
    parser.add_argument("--no-auto-start", dest="auto_start", action="store_false")
    parser.add_argument("--start-timeout", type=float, default=120.0)
    parser.add_argument("--poll-timeout", type=float, default=600.0)
    parser.add_argument("--poll-interval", type=float, default=2.0)
    parser.add_argument("--workflow", default=DEFAULT_WORKFLOW_PATH,
                         help="Single-pass / hero-pass workflow (no IPAdapter)")
    parser.add_argument("--mask-dilate", type=int, default=4)
    parser.add_argument("--no-ipadapter", dest="use_ipadapter", action="store_false",
                         help="Restore Milestone-6 single-pass behavior (every frame "
                              "independent, no IPAdapter reference conditioning)")
    parser.add_argument("--ipadapter-workflow", default=DEFAULT_IPADAPTER_WORKFLOW_PATH,
                         help="Two-pass workflow used for pass 2 (ControlNet-Depth + IPAdapter)")
    parser.add_argument("--ipadapter-weight", type=float, default=DEFAULT_IPADAPTER_WEIGHT,
                         help="IPAdapter conditioning strength (0-1ish); higher = more "
                              "faithful to the reference image, lower = more prompt/pose freedom")
    parser.add_argument("--reference", default=None,
                         help="Explicit reference image path for IPAdapter conditioning; "
                              "skips pass 1 (hero-frame generation) entirely. Also the hook "
                              "for a future fixed character-design image.")
    parser.add_argument("--hero-frame", type=int, default=None,
                         help="Frame index to use as the pass-1 hero frame (default: the "
                              "middle frame of the requested set). Ignored if --reference is set.")
    args = parser.parse_args()

    stylize_frames(
        input_dir=args.input,
        output_dir=args.output,
        prompt_theme=args.prompt_theme,
        negative_prompt=args.negative_prompt,
        seed=args.seed,
        steps=args.steps,
        cfg=args.cfg,
        sampler_name=args.sampler,
        scheduler=args.scheduler,
        controlnet_strength=args.controlnet_strength,
        width=args.width,
        height=args.height,
        frame_indices=_parse_frames_arg(args.frames),
        comfyui_url=args.comfyui_url,
        auto_start=args.auto_start,
        comfyui_dir=args.comfyui_dir,
        comfyui_python=args.comfyui_python,
        start_timeout=args.start_timeout,
        poll_timeout=args.poll_timeout,
        poll_interval=args.poll_interval,
        workflow_path=args.workflow,
        mask_dilate_px=args.mask_dilate,
        use_ipadapter=args.use_ipadapter,
        ipadapter_workflow_path=args.ipadapter_workflow,
        ipadapter_weight=args.ipadapter_weight,
        reference_image_path=args.reference,
        hero_frame_index=args.hero_frame,
    )


if __name__ == "__main__":
    main()
