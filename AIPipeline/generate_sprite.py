"""
Autonomous AI 2D sprite pipeline orchestrator (Phase 6.4).

Wires together the four REAL pipeline stages into one end-to-end run:

  1. text_to_motion.py   -- prompt -> .bvh   (MoMask, MPS)
  2. blender_render.py   -- .bvh -> beauty/depth frame sequences (Blender headless)
  3. comfy_client.py     -- depth+beauty frames -> stylized transparent sprite frames (ComfyUI SDXL+ControlNet)
  4. pack_sprites.py     -- stylized frames -> single sprite sheet + JSON sidecar
  5. make_preview_gif.py -- stylized frames -> animated preview GIF

Every stage is a real subprocess invocation of an already-verified script;
there is no mock/fallback anywhere in this file. Any stage failing
(non-zero exit) aborts the whole run immediately with a clear message.

Usage:
    python3 AIPipeline/generate_sprite.py "heavy sword swing"
    python3 AIPipeline/generate_sprite.py "heavy sword swing" --skip-to stylize
    python3 AIPipeline/generate_sprite.py "a person walks forward" --prop none

Resuming a partial run: pass --skip-to {render,stylize,pack,preview} to
reuse existing temp/<run_name>/ outputs from a previous run instead of
re-paying for earlier stages (valuable given the ComfyUI stylize stage
takes ~140s/frame).
"""
import argparse
import os
import re
import shutil
import subprocess
import sys
import time

PIPELINE_DIR = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.dirname(PIPELINE_DIR)
SRC_DIR = os.path.join(PIPELINE_DIR, "src")
VENV_PYTHON = os.path.join(PIPELINE_DIR, ".venv", "bin", "python")
BLENDER_BIN = "/Applications/Blender.app/Contents/MacOS/Blender"

STAGES = ["motion", "render", "stylize", "colormatch", "pack", "preview"]

CHARACTER_REFS_DIR = os.path.join(PIPELINE_DIR, "character_refs")

# Simple keyword heuristic: if the prompt reads as weapon-themed, default
# to attaching the sword prop in the Blender render stage (--prop can
# still override this either way).
WEAPON_KEYWORDS = ("sword", "axe", "blade", "weapon", "mace", "dagger", "spear", "hammer")


def slugify(prompt: str) -> str:
    s = prompt.strip().lower()
    s = re.sub(r"[^a-z0-9]+", "_", s)
    s = re.sub(r"_+", "_", s).strip("_")
    return s or "sprite"


def resolve_prop(prompt: str, prop_arg: str):
    """Returns the --prop value to pass to blender_render.py, or None."""
    if prop_arg is None:
        auto = any(k in prompt.lower() for k in WEAPON_KEYWORDS)
        return "sword" if auto else None
    if prop_arg == "none":
        return None
    return prop_arg


def resolve_character_reference(prompt: str, explicit_ref: str | None) -> str | None:
    """Returns a --reference path for comfy_client.py, or None (falls back
    to hero-frame auto-pick). Explicit --reference always wins."""
    if explicit_ref:
        if os.path.isfile(explicit_ref):
            return explicit_ref
        
        name = explicit_ref
        if not name.lower().endswith(".png"):
            name += ".png"
        
        resolved_path = os.path.join(CHARACTER_REFS_DIR, name)
        if os.path.isfile(resolved_path):
            return resolved_path
            
        return explicit_ref
    if not os.path.isdir(CHARACTER_REFS_DIR):
        return None
    slug = slugify(prompt)
    for fname in os.listdir(CHARACTER_REFS_DIR):
        name, ext = os.path.splitext(fname)
        if ext.lower() == ".png" and name in slug:
            return os.path.join(CHARACTER_REFS_DIR, fname)
    return None


def run_stage(label: str, cmd: list, cwd: str = None):
    print(f"\n>>> {label}")
    print(f"    $ {' '.join(cmd)}")
    t0 = time.time()
    try:
        subprocess.run(cmd, cwd=cwd, check=True)
    except subprocess.CalledProcessError as exc:
        elapsed = time.time() - t0
        print(f"\n[Pipeline] STAGE FAILED: {label} (exit {exc.returncode}) after {elapsed:.1f}s",
              file=sys.stderr)
        raise
    except FileNotFoundError as exc:
        print(f"\n[Pipeline] STAGE FAILED: {label} -- executable not found: {exc}",
              file=sys.stderr)
        raise
    elapsed = time.time() - t0
    print(f"[Pipeline] {label} done in {elapsed:.1f}s")
    return elapsed


def require_dir_with(pattern_desc, directory, predicate):
    if not os.path.isdir(directory) or not predicate(directory):
        raise RuntimeError(
            f"--skip-to requires existing {pattern_desc} in {directory}, but "
            f"none were found. Run without --skip-to (or an earlier stage) first."
        )


def has_files_matching(directory, prefix, suffix=".png"):
    if not os.path.isdir(directory):
        return False
    return any(f.startswith(prefix) and f.endswith(suffix) for f in os.listdir(directory))


def run_pipeline(args):
    prompt = args.prompt
    run_name = slugify(prompt)
    prop = resolve_prop(prompt, args.prop)
    args.reference = resolve_character_reference(prompt, args.character or args.reference)

    run_root = os.path.join(PIPELINE_DIR, "temp", run_name)
    bvh_path = os.path.join(run_root, "motion.bvh")
    render_dir = os.path.join(run_root, "renders")
    style_dir = os.path.join(run_root, "stylized")
    color_dir = os.path.join(run_root, "colormatch")

    unity_sprites_dir = os.path.join(PROJECT_ROOT, "Assets", "Art", "Sprites", "Generated")
    sheet_path = os.path.join(unity_sprites_dir, f"{run_name}_Sheet.png")
    gif_path = os.path.join(PIPELINE_DIR, "Previews", f"{run_name}_preview.gif")

    skip_idx = STAGES.index(args.skip_to)

    print("=== Autonomous AI 2D Sprite Pipeline ===")
    print(f"Prompt: {prompt}")
    print(f"Run name: {run_name}")
    print(f"Prop: {prop or '(none)'}")
    print(f"Frames: {args.frames}  Res: {args.res}  Seed: {args.seed}  Cell: {args.cell}")
    print(f"Lock facing: {args.lock_facing}  IPAdapter: {not args.no_ipadapter}"
          f"{' (ref=' + args.reference + ')' if args.reference else ''}")
    print(f"Temp dir: {run_root}")
    print(f"Resuming from stage: {args.skip_to}" if skip_idx > 0 else "Running all stages")

    os.makedirs(unity_sprites_dir, exist_ok=True)
    os.makedirs(os.path.join(PIPELINE_DIR, "Previews"), exist_ok=True)

    if skip_idx == 0:
        # Fresh run: wipe this run's temp dir so stale frames from a
        # previous attempt at the same prompt can't leak into the new sheet.
        if os.path.exists(run_root):
            shutil.rmtree(run_root)
    os.makedirs(run_root, exist_ok=True)

    timings = {}
    total_t0 = time.time()

    # --- Stage 1: Text -> Motion (BVH) ---------------------------------
    if skip_idx <= STAGES.index("motion"):
        timings["motion"] = run_stage(
            "STAGE 1: Text -> Motion (MoMask, MPS)",
            ["python3", os.path.join(SRC_DIR, "text_to_motion.py"),
             "--prompt", prompt, "--output", bvh_path, "--seed", str(args.seed)],
        )
    else:
        if not os.path.isfile(bvh_path):
            raise RuntimeError(f"--skip-to {args.skip_to} requires an existing BVH at {bvh_path}")
        print(f"\n>>> STAGE 1: skipped (reusing {bvh_path})")

    # --- Stage 2: Blender render (beauty + depth frames) ---------------
    if skip_idx <= STAGES.index("render"):
        os.makedirs(render_dir, exist_ok=True)
        blender_cmd = [
            BLENDER_BIN, "-b", "-P", os.path.join(SRC_DIR, "blender_render.py"),
            "--", bvh_path, render_dir,
            "--frames", str(args.frames), "--res", str(args.res),
        ]
        if prop:
            blender_cmd += ["--prop", prop]
        if args.lock_facing:
            blender_cmd += ["--lock-facing"]
        timings["render"] = run_stage("STAGE 2: Blender render (beauty + depth)", blender_cmd)
    else:
        if not (has_files_matching(render_dir, "frame_") and has_files_matching(render_dir, "depth_")):
            raise RuntimeError(
                f"--skip-to {args.skip_to} requires existing frame_*/depth_* renders in {render_dir}"
            )
        print(f"\n>>> STAGE 2: skipped (reusing {render_dir})")

    # --- Stage 3: ComfyUI stylization -----------------------------------
    if skip_idx <= STAGES.index("stylize"):
        comfy_cmd = [
            VENV_PYTHON, os.path.join(SRC_DIR, "comfy_client.py"),
            "--input", render_dir, "--output", style_dir, "--seed", str(args.seed),
        ]
        if args.no_ipadapter:
            comfy_cmd += ["--no-ipadapter"]
        if args.reference:
            comfy_cmd += ["--reference", args.reference]
        if args.ipadapter_weight is not None:
            comfy_cmd += ["--ipadapter-weight", str(args.ipadapter_weight)]
        if args.hero_frame is not None:
            comfy_cmd += ["--hero-frame", str(args.hero_frame)]
        timings["stylize"] = run_stage(
            "STAGE 3: ComfyUI stylization (SDXL + ControlNet-Depth)",
            comfy_cmd,
        )
    else:
        if not has_files_matching(style_dir, "frame_"):
            raise RuntimeError(
                f"--skip-to {args.skip_to} requires existing stylized frame_*.png in {style_dir}"
            )
        print(f"\n>>> STAGE 3: skipped (reusing {style_dir})")

    # --- Stage 3.5: Color match -----------------------------------------
    if skip_idx <= STAGES.index("colormatch"):
        if not args.no_color_match:
            os.makedirs(color_dir, exist_ok=True)
            # Find the hero frame (or external reference) to match against.
            # If explicit reference is provided, use it. Otherwise, use hero pass output from style_dir.
            # We look for hero_*.png in stylized/hero/ or use args.reference
            if args.reference:
                match_ref = args.reference
            else:
                hero_dir = os.path.join(style_dir, "hero")
                match_ref = next((os.path.join(hero_dir, f) for f in os.listdir(hero_dir) if f.startswith("hero_") and f.endswith(".png")), None) if os.path.isdir(hero_dir) else None
                if not match_ref:
                    # Fallback to middle frame if no hero explicitly generated (e.g., no_ipadapter)
                    frames = sorted([f for f in os.listdir(style_dir) if f.startswith("frame_") and f.endswith(".png")])
                    if frames:
                        match_ref = os.path.join(style_dir, frames[len(frames) // 2])

            if match_ref:
                timings["colormatch"] = run_stage(
                    "STAGE 3.5: Post-hoc Color Match",
                    [VENV_PYTHON, os.path.join(SRC_DIR, "color_consistency.py"),
                     "--input", style_dir, "--output", color_dir,
                     "--reference", match_ref],
                )
            else:
                print("\n>>> STAGE 3.5: skipped (no reference frame found to match against)")
                color_dir = style_dir # Fallback
        else:
            print("\n>>> STAGE 3.5: skipped (--no-color-match)")
            color_dir = style_dir # Bypass
    else:
        if not has_files_matching(color_dir, "frame_"):
            # It might have been skipped previously, fallback to style_dir
            if has_files_matching(style_dir, "frame_"):
                color_dir = style_dir
            else:
                raise RuntimeError(
                    f"--skip-to {args.skip_to} requires existing stylized frame_*.png in {color_dir} or {style_dir}"
                )
        print(f"\n>>> STAGE 3.5: skipped (reusing {color_dir})")

    # --- Stage 4: Pack sprite sheet --------------------------------------
    if skip_idx <= STAGES.index("pack"):
        timings["pack"] = run_stage(
            "STAGE 4: Packing sprite sheet",
            [VENV_PYTHON, os.path.join(SRC_DIR, "pack_sprites.py"),
             "--input", color_dir, "--output", sheet_path,
             "--cell", str(args.cell), "--cols", args.cols],
        )
    else:
        if not os.path.isfile(sheet_path):
            raise RuntimeError(f"--skip-to {args.skip_to} requires an existing sheet at {sheet_path}")
        print(f"\n>>> STAGE 4: skipped (reusing {sheet_path})")

    # --- Stage 5: Preview GIF --------------------------------------------
    timings["preview"] = run_stage(
        "STAGE 5: Generating preview GIF",
        [VENV_PYTHON, os.path.join(SRC_DIR, "make_preview_gif.py"),
         "--input", color_dir, "--output", gif_path],
    )

    total_elapsed = time.time() - total_t0

    print("\n=== Pipeline Complete! ===")
    print(f"Sprite Sheet: {sheet_path}")
    print(f"Sheet metadata: {sheet_path}.meta.json")
    print(f"Preview GIF: {gif_path}")
    print("\n--- Timings ---")
    for stage in STAGES:
        if stage in timings:
            print(f"  {stage:10s}: {timings[stage]:.1f}s")
    print(f"  {'TOTAL':10s}: {total_elapsed:.1f}s")
    print(
        "\nSwitch to Unity. AutoSpriteImporter will auto-slice the sheet using "
        "the JSON sidecar and can generate an AnimationClip from it."
    )


def main():
    parser = argparse.ArgumentParser(description="Autonomous 2D AI Sprite Generator")
    parser.add_argument("prompt", type=str, help="The action to generate (e.g., 'heavy sword swing')")
    parser.add_argument("--frames", type=int, default=16, help="Frame count (default 16)")
    parser.add_argument("--seed", type=int, default=12345, help="RNG seed for motion + stylization (default 12345)")
    parser.add_argument("--res", type=int, default=1024, help="Blender render resolution (default 1024)")
    parser.add_argument("--cell", type=int, default=512, help="Sprite sheet cell size in px (default 512)")
    parser.add_argument("--cols", default="auto", help="Sprite sheet columns, or 'auto' (default)")
    parser.add_argument("--prop", choices=["sword", "none"], default=None,
                         help="Force a Blender prop on/off. Default: auto-detect from the prompt "
                              "(sword/axe/weapon keyword -> 'sword').")
    parser.add_argument("--lock-facing", dest="lock_facing", action="store_true", default=True,
                         help="Cancel root-bone yaw drift in the Blender render so the "
                              "character's side-profile facing stays constant across all "
                              "frames (default: ON -- side-scroller sprites want a stable "
                              "profile). Pass --no-lock-facing for actions where the "
                              "character should visibly turn, e.g. a turning attack.")
    parser.add_argument("--no-lock-facing", dest="lock_facing", action="store_false",
                         help="Disable facing-lock (restores free root yaw). Use for "
                              "actions that should show the character turning, e.g. a "
                              "turning attack.")
    parser.add_argument("--no-ipadapter", dest="no_ipadapter", action="store_true", default=False,
                         help="Restore Milestone-6 single-pass stylization (every frame "
                              "independent, no IPAdapter reference conditioning). Default: "
                              "off -- comfy_client's two-pass IPAdapter-consistent mode is "
                              "used by default.")
    parser.add_argument("--no-color-match", dest="no_color_match", action="store_true", default=False,
                         help="Disable post-hoc LAB color matching pass (Stage 3.5).")
    parser.add_argument("--character", default=None,
                         help="Explicit character reference name/slug to override auto-detection.")
    parser.add_argument("--reference", default=None,
                         help="Explicit reference image path for IPAdapter conditioning "
                              "(passed through to comfy_client.py); skips hero-frame "
                              "generation entirely.")
    parser.add_argument("--ipadapter-weight", type=float, default=None,
                         help="IPAdapter conditioning strength (passed through to "
                              "comfy_client.py; its own default is 0.8 if unset here).")
    parser.add_argument("--hero-frame", type=int, default=None,
                         help="Frame index to use as the pass-1 hero frame (passed through "
                              "to comfy_client.py; default there is the middle frame). "
                              "Ignored if --reference is set.")
    parser.add_argument("--skip-to", choices=STAGES, default="motion",
                         help="Resume from a stage, reusing existing temp/<run_name>/ outputs "
                              "for earlier stages (default: run everything from 'motion').")
    args = parser.parse_args()

    try:
        run_pipeline(args)
    except (subprocess.CalledProcessError, RuntimeError, FileNotFoundError) as exc:
        print(f"\n[Pipeline] ABORTED: {exc}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()
