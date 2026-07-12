---
gsd_state_version: 1.0
milestone: v8.0
milestone_name: Playable Scenes & Prototype Integration
status: planning
last_updated: "2026-07-12T17:36:34.754Z"
last_activity: 2026-07-12 — Roadmap created for milestone v8.0
progress:
  total_phases: 3
  completed_phases: 0
  total_plans: 0
  completed_plans: 0
  percent: 0
---

# NAOR - Project State

## Current Phase

- Milestone v7.0 closed. Pipeline is real, consistent-by-default, and disaster-recoverable (`AIPipeline/setup_ai_tools.sh`).

## Recent Decisions

- AI toolchain lives at `game_NAOR/AI_Tools` (gitignored; user's placement choice), env override NAOR_AI_TOOLS_DIR; anything outside git must be recreatable by the setup script.
- Consistent mode (IPAdapter two-pass + facing lock) is the pipeline default; escape hatches: --no-ipadapter, --no-lock-facing, --reference for curated character designs.

## Pending Action

- Review roadmap and proceed to planning for Phase 8.1.

## Current Position

Phase: 8.1 (Prototype Scene Consolidation)
Plan: —
Status: Ready for Phase 8.1 Planning
Last activity: 2026-07-12 — Roadmap created for milestone v8.0
