import os
import subprocess
import sys
import time

# MoMask (text-to-motion) is a separate research repo/venv living outside
# version control (it's ~1.6GB with checkpoints -- see AIPipeline/setup_ai_tools.sh
# for how it's provisioned). See: https://github.com/EricGuo5513/momask-codes
NAOR_AI_TOOLS_DIR = os.environ.get(
    "NAOR_AI_TOOLS_DIR", "/Volumes/aebasol_1tb/Ob/Projects/game_NAOR/AI_Tools"
)
MOMASK_DIR = os.path.join(NAOR_AI_TOOLS_DIR, "MoMask")
MOMASK_PYTHON = os.path.join(MOMASK_DIR, "venv", "bin", "python3")
# naor_generate_bvh.py is a small script added to the MoMask checkout by the
# NAOR project (not part of upstream momask-codes) -- see
# .planning/phases/6.1/SUMMARY.md for what it does and why it exists instead
# of shelling out to MoMask's own gen_t2m.py.
MOMASK_GEN_SCRIPT = os.path.join(MOMASK_DIR, "naor_generate_bvh.py")

DEFAULT_MOTION_LENGTH = 96  # frames @ 20fps == 4.8s


def generate_motion(
    prompt: str,
    output_path: str,
    motion_length: int = DEFAULT_MOTION_LENGTH,
    device: str = "auto",
    seed: int = 10107,
    foot_ik: bool = False,
    timeout: int = 900,
) -> str:
    """
    Generates a real .bvh motion file from a text prompt using MoMask
    (text-to-motion AI, https://github.com/EricGuo5513/momask-codes),
    invoked as a subprocess in MoMask's own venv (which has the pinned deps
    this 2023 research code needs -- separate from this repo's venv).

    Uses Apple Silicon MPS acceleration when available, automatically
    retrying on CPU if MPS raises an error (handled inside
    naor_generate_bvh.py). Motion generation is light (a handful of small
    transformers), so CPU fallback is still fast.

    Args:
        prompt: natural-language motion description, e.g.
            "a person swings a heavy sword downward"
        output_path: where to write the resulting .bvh file
        motion_length: frames at 20fps (default 96 == ~4.8s). Pass 0 to let
            MoMask estimate a length from the text itself.
        device: 'auto' (try mps, fall back to cpu), 'mps', or 'cpu'
        seed: RNG seed for reproducibility
        foot_ik: whether to run MoMask's foot-contact IK cleanup pass
            (slower; not needed for stylized game motion)
        timeout: subprocess timeout in seconds

    Returns:
        output_path, on success. Raises subprocess.CalledProcessError (with
        MoMask's stderr attached) if generation fails -- no mock/fallback
        file is ever written.
    """
    if not os.path.isfile(MOMASK_PYTHON):
        raise FileNotFoundError(
            f"MoMask venv python not found at {MOMASK_PYTHON}. "
            f"Is MoMask installed at {MOMASK_DIR}?"
        )
    if not os.path.isfile(MOMASK_GEN_SCRIPT):
        raise FileNotFoundError(
            f"naor_generate_bvh.py not found at {MOMASK_GEN_SCRIPT}. "
            f"It should have been added inside the MoMask checkout."
        )

    output_path = os.path.abspath(output_path)
    os.makedirs(os.path.dirname(output_path), exist_ok=True)

    command = [
        MOMASK_PYTHON,
        MOMASK_GEN_SCRIPT,
        "--text_prompt", prompt,
        "--output_bvh", output_path,
        "--motion_length", str(motion_length),
        "--device", device,
        "--seed", str(seed),
    ]
    if foot_ik:
        command.append("--foot_ik")

    print(f"[Text-to-Motion] Generating 3D animation for prompt: '{prompt}'")
    print(f"[Text-to-Motion] Invoking MoMask ({MOMASK_DIR}) via subprocess...")

    start = time.time()
    result = subprocess.run(
        command,
        cwd=MOMASK_DIR,
        capture_output=True,
        text=True,
        timeout=timeout,
    )
    elapsed = time.time() - start

    # Surface MoMask's own progress/log lines (it logs to stderr).
    if result.stderr:
        print(result.stderr)

    if result.returncode != 0:
        print(result.stdout)
        raise subprocess.CalledProcessError(
            result.returncode, command, output=result.stdout, stderr=result.stderr
        )

    if not os.path.isfile(output_path) or os.path.getsize(output_path) == 0:
        raise RuntimeError(
            f"MoMask reported success but no BVH was written to {output_path}"
        )

    print(f"[Text-to-Motion] Saved motion data to {output_path} ({elapsed:.1f}s)")
    return output_path


if __name__ == "__main__":
    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument("--prompt", required=True)
    parser.add_argument("--output", default="AIPipeline/temp/output.bvh")
    parser.add_argument("--motion_length", type=int, default=DEFAULT_MOTION_LENGTH,
                         help="Frames at 20fps (default 96 = ~4.8s). 0 = auto-estimate from text.")
    parser.add_argument("--device", default="auto", choices=["auto", "mps", "cpu"])
    parser.add_argument("--seed", type=int, default=10107)
    parser.add_argument("--foot_ik", action="store_true")
    args = parser.parse_args()

    try:
        generate_motion(
            args.prompt,
            args.output,
            motion_length=args.motion_length,
            device=args.device,
            seed=args.seed,
            foot_ik=args.foot_ik,
        )
    except subprocess.CalledProcessError as e:
        print(f"[Text-to-Motion] MoMask generation FAILED (exit {e.returncode}).", file=sys.stderr)
        sys.exit(1)
