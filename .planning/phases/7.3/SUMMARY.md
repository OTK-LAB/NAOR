# Phase 7.3 — E2E Consistency Validation (+7.3b Toolchain Recovery)

**Status:** ✅ Complete (commits e43ede4, afc2b82; final sheet packed 2026-07-12)

## Wiring
`generate_sprite.py`: `--lock-facing` default ON (`--no-lock-facing` for turning attacks), pass-throughs for `--no-ipadapter/--reference/--ipadapter-weight/--hero-frame`; IPAdapter consistent mode is the pipeline default.

## Incident (7.3b)
The E2E validation run was killed at frame 11/16: the user manually deleted the old `/Volumes/aebasol_1tb/Ob/AI_Tools` install (~12GB) mid-run. Recovery hardening: committed idempotent `AIPipeline/setup_ai_tools.sh` re-provisions the full toolchain into `NAOR_AI_TOOLS_DIR` (new default: `game_NAOR/AI_Tools`, gitignored); the MoMask BVH helper now has its canonical copy in-repo (`AIPipeline/src/momask_helper/`). Run resumed cheaply via salvaged hero + `--reference` for frames 11-15, then `--skip-to pack`.

## Results (full 16-frame sheet, same metric/script)
| | M6 baseline | M7 consistent | Δ |
|---|---|---|---|
| Mean pairwise hue/sat distance | 0.2237 | 0.0780 | **-65%** |

Visual (orchestrator-inspected): one grey-steel knight across all 16 cells (M6 had 3+ distinct characters), sword in every frame, coherent swing progression, no back-view frames (M6 had 2), transparent bg intact. Sheet/sidecar/GIF replaced at the same asset paths.
