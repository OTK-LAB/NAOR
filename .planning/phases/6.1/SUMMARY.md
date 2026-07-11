# Phase 6.1 — Real MoMask Text→BVH Generation

**Status:** ✅ Complete (verified independently by orchestrator, 2026-07-11)

## What was done
Replaced the mock `AIPipeline/src/text_to_motion.py` with a real implementation. It subprocess-invokes MoMask's own venv python running a new helper (`/Volumes/aebasol_1tb/Ob/AI_Tools/MoMask/naor_generate_bvh.py`, lives in the MoMask checkout, not this repo) that loads the RVQ-VAE + mask transformer + residual transformer + length estimator and converts generated joints to BVH via `Joint2BVHConvertor` (pure-numpy IK path, no CUDA needed).

## Working command
```bash
python3 AIPipeline/src/text_to_motion.py --prompt "a person swings a heavy sword downward" --output AIPipeline/temp/test_sword.bvh
```
Optional flags: `--motion_length` (default 96 ≈ 4.8s @20fps), `--device` (auto→mps), `--seed`, `--foot_ik`.

## Measured
~5s wall-clock per generation on MPS (after one-time ~354MB CLIP download to `~/.cache/clip/`). Output: 22-joint HumanML3D skeleton, 96 frames, ~66KB BVH, verified non-degenerate motion.

## Patches applied inside the MoMask checkout (all tagged `# NAOR patch`)
Python 3.14 venv kept; only deprecated-numpy fixes were needed:
- `common/quaternion.py`: `np.finfo(np.float)` → `np.finfo(float)`
- `utils/motion_process.py`, `visualization/remove_fs.py`: `.astype(np.float)` → `.astype(float)`
- `visualization/AnimationStructure.py`: `.astype(np.int)` → `.astype(int)`
- `visualization/Animation.py`: removed `numpy.core.umath_tests` import; `ut.matrix_multiply` → `np.matmul`

## Known limitations / risks
- Prompts produce body motion only — no sword/prop in the skeleton (HumanML3D has no weapons). Weapon must be handled downstream (prompt-side in SDXL or a bone-attached prop in Blender later).
- Hardcoded to t2m (22-joint) checkpoints; KIT set unused.
- `glove/` absent — confirmed unnecessary (text conditioning is CLIP-only on this path).
- `MOMASK_DIR` hardcoded to external SSD path; fails fast if unmounted.
