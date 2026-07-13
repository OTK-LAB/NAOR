---
gsd_state_version: 1.0
milestone: v8.0
milestone_name: Playable Scenes & Prototype Integration
status: executing
last_updated: "2026-07-12T23:30:00.000Z"
last_activity: 2026-07-12 — Phase 8.1 execution completed manually
progress:
  total_phases: 3
  completed_phases: 0
  total_plans: 1
  completed_plans: 1
  percent: 100
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

Phase: 8.1 (Prototype Scene Consolidation)
Plan: 08.1
Status: Execution Complete
Last activity: 2026-07-12 — Phase 8.1 execution completed manually

## Accumulated Context

### Roadmap Evolution
- Phase 9 added: AI Pipeline v2.0 Upgrades
