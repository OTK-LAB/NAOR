---
gsd_state_version: 1.0
milestone: v6.0
milestone_name: Pipeline Reality
current_phase: 6.4
current_phase_name: Real sprite packing + E2E run
status: phases_complete_uat_pending
last_activity: 2026-07-12
last_activity_desc: All 4 phases (+6.2b) complete and committed; first full E2E run produced a real sprite sheet in 37.4 min. Pending user UAT in Unity editor (AutoSpriteImporter slice check).
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
- User UAT in Unity editor, then /gsd-audit-milestone + /gsd-complete-milestone for v6.0.
