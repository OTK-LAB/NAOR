---
gsd_state_version: 1.0
milestone: v8.0
milestone_name: Playable Scenes & Prototype Integration
current_phase: 8.2
status: completed
last_updated: "2026-07-14T11:50:08.723Z"
last_activity: 2026-07-14
last_activity_desc: Phase 8.2 marked complete
progress:
  total_phases: 4
  completed_phases: 3
  total_plans: 5
  completed_plans: 4
  percent: 75
current_phase_name: ai-pipeline-v2-0-upgrades
---

# NAOR - Project State

## Current Phase

- Milestone v7.0 closed. Pipeline is real, consistent-by-default, and disaster-recoverable (`AIPipeline/setup_ai_tools.sh`).

## Recent Decisions

- AI toolchain lives at `game_NAOR/AI_Tools` (gitignored; user's placement choice), env override NAOR_AI_TOOLS_DIR; anything outside git must be recreatable by the setup script.
- Consistent mode (IPAdapter two-pass + facing lock) is the pipeline default; escape hatches: --no-ipadapter, --no-lock-facing, --reference for curated character designs.

## Pending Action

- Verify Phase 8.1 (prototype-scene-consolidation).

## Current Position

Phase: 8.2 — COMPLETE
Plan: 1 of 1
Status: Phase 8.2 complete
Last activity: 2026-07-14 — Phase 8.2 marked complete

## Accumulated Context

### Roadmap Evolution

- Phase 9 added: AI Pipeline v2.0 Upgrades
