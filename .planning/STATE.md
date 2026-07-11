---
gsd_state_version: 1.0
milestone: v6.0
milestone_name: Pipeline Reality
current_phase: 6.1
current_phase_name: Real MoMask text-to-BVH generation
status: in_progress
last_activity: 2026-07-11
last_activity_desc: Milestone 6 started — replacing mocked pipeline modules with real implementations (Fable orchestrating, Sonnet subagents coding)
progress:
  total_phases: 4
  completed_phases: 0
  total_plans: 0
  completed_plans: 0
  percent: 0
---

# NAOR - Project State

## Current Phase
- Phase 6.1 (Real MoMask text→BVH) — in progress, parallel with 6.2 (Blender render)

## Recent Decisions
- 2026-07-11 audit found v4.0 pipeline was 100% mock (all 5 modules stubs, empty workflows/). Milestone 6 replaces mocks with real code.
- Milestone 5 (model downloads) confirmed genuinely complete: SDXL 6.5G, ControlNet Depth 2.3G, T2I OpenPose 151M, BiRefNet 424M, MoMask checkpoints all on external SSD.
- Orchestration model: Claude Fable 5 manages/verifies; Sonnet subagents implement per-phase.
- Temporal consistency (AnimateDiff) deferred: start with per-frame img2img, fixed seed + ControlNet Depth; revisit after first real output.

## Pending Action
- Phase 6.1 + 6.2 executing in parallel; 6.3 (ComfyUI) after, to avoid MPS memory contention; 6.4 E2E last.
