# Phase 7.2 — Facing Lock in Blender Render

**Status:** ✅ Complete (commit c84d90d)

`--lock-facing` in `blender_render.py`: per sampled frame, computes the Hips bone's ground-plane heading and counter-rotates the armature object (pivot at hips) so facing stays constant at frame-0's heading. Composes with the sword prop and camera auto-framing (bounds recomputed post-correction). Non-accumulating (matrix reset each frame); falls back gracefully on unknown rigs.

**Measured:** root yaw drift 21° → 0.000° across sampled frames; no-flag path bit-identical to previous baseline; negligible render-time impact.

**Tradeoff:** turning attacks (spins) get flattened — pass `--no-lock-facing` for those (orchestrator default is ON).
