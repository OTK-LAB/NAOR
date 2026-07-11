# Phase 6.2 — Real Blender Headless BVH→Frames Render

**Status:** ✅ Complete (verified visually by orchestrator on real MoMask output, 2026-07-12)

## What was done
Replaced the mock `AIPipeline/src/blender_render.py` with a real ~640-line implementation. Imports a BVH, builds a connected humanoid body (capsule per bone + icosphere joint balls + bridging capsules across parent→child gaps, skinned via vertex groups + Armature modifier), renders per-frame beauty (matte gray, transparent bg) + depth (ControlNet convention: bright near figure on pure black, full 0→255 range) with EEVEE at 1024×1024.

Also added `AIPipeline/src/make_test_bvh.py` — dependency-free generator of a 22-joint SMPL-named walk BVH for testing without MoMask.

## Working commands
```bash
python3 AIPipeline/src/make_test_bvh.py AIPipeline/temp/test_walk.bvh
/Applications/Blender.app/Contents/MacOS/Blender -b -P AIPipeline/src/blender_render.py -- <bvh> <outdir> --frames 16 --res 1024
```

## Measured
~5s wall-clock for 16 frame-pairs (0.3s/pair), both on synthetic walk and real MoMask `test_sword.bvh` (22-joint HumanML3D). Auto-framed ortho side camera keeps figure fully in frame every frame.

## Key technical notes
- Blender 5.1 compositor File Output nodes never fire headlessly (API moved to `scene.compositing_node_group`); depth is instead extracted via Camera Data → Map Range → Emission material with `view_layer.material_override`, normalized per frame to the figure's own camera-space near/far.
- Bone-thickness heuristics are case-insensitive substring rules tolerant of SMPL and generic rig names; unknown rigs fall back to uniform capsules (never crashes).

## Limitations / follow-ups
- `generate_sprite.py` still invokes this stage via `python3` — must be updated to the Blender binary invocation (Phase 6.4).
- Depth normalization is per-frame (MiDaS-style) — not temporally consistent across frames; fine for ControlNet.
- Depth PNG is sRGB-encoded; ControlNet normalizes so OK, use 16-bit linear if ever needed downstream.
- Camera facing heuristic could pick a front view for motions with ambiguous action planes; sword clip picked correctly.
- Hands/feet are stylized stubs — intentional, SDXL+ControlNet re-interprets the silhouette.
