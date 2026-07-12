---
gsd_state_version: 1.0
milestone: v7.0
milestone_name: Consistency
current_phase: 7.3
current_phase_name: E2E consistency validation
status: completed
last_activity: 2026-07-12
last_activity_desc: Milestone v7.0 closed — identity flicker -65% on full sheet, facing flips gone, toolchain now reproducibly provisioned. Tagged v7.0.
progress:
  total_phases: 3
  completed_phases: 3
  total_plans: 0
  completed_plans: 0
  percent: 100
---

# NAOR - Project State

## Current Phase
- Milestone v7.0 closed. Pipeline is real, consistent-by-default, and disaster-recoverable (`AIPipeline/setup_ai_tools.sh`).

## Recent Decisions
- AI toolchain lives at `game_NAOR/AI_Tools` (gitignored; user's placement choice), env override NAOR_AI_TOOLS_DIR; anything outside git must be recreatable by the setup script.
- Consistent mode (IPAdapter two-pass + facing lock) is the pipeline default; escape hatches: --no-ipadapter, --no-lock-facing, --reference for curated character designs.

## Pending Action
- Next milestone TBD with user. Candidates: return to game milestones (v1.0 refactor backlog, v2.0 UI, v3.0 scenes/story — pipeline can now feed them), or further art quality (curated character reference set, proxy-body anatomy, AnimateDiff-class smoothing).
