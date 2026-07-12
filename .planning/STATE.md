---
gsd_state_version: 1.0
milestone: v6.0
milestone_name: Pipeline Reality
current_phase: 6.4
current_phase_name: Real sprite packing + E2E run
status: completed
last_activity: 2026-07-12
last_activity_desc: Milestone v6.0 closed — audit PASSED, user UAT confirmed 16 auto-sliced sprites in Unity. Tagged v6.0.
progress:
  total_phases: 4
  completed_phases: 4
  total_plans: 0
  completed_plans: 0
  percent: 100
---

# NAOR - Project State

## Current Phase
- Milestone 6 implementation complete. Pending: user UAT (open Unity, confirm AutoSpriteImporter slices heavy_sword_swing_Sheet.png into 16 sprites + AnimationClip path), then milestone audit/close.

## Recent Decisions
- Sword handled as Blender hand-bone prop (`--prop sword`, auto-heuristic on weapon keywords) instead of prompt-only — fixes cutout amputation and pins weapon placement for ControlNet.
- Background removal via beauty-frame alpha cutout (4px dilation), not BiRefNet — zero extra models/nodes.
- Pipeline stage costs (measured): MoMask 7s, Blender 5s, SDXL stylize ~137s/frame (99% of cost), pack+GIF <1s.

## Known issues carried to next milestone
- Frame-to-frame character identity flicker (independent SDXL generations) → IPAdapter / img2img chaining / LoRA candidate fixes.
- Occasional back-view frames (MoMask rotates character) → facing constraint needed.
- AutoSpriteImporter uses deprecated TextureImporter.spritesheet API.

## Pending Action
- Milestone v6.0 closed. Next milestone TBD with user — candidates: v7.0 Consistency (IPAdapter/img2img-chain/LoRA + facing constraint) or return to game milestones (v1.0 refactor backlog, v2.0 UI, v3.0 scenes/story).
