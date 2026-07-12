# NAOR - Project Context

## Overview
This project is a revival of an old 2D animated game project. It had become disorganized over time, and this fresh start (`Efe-Basol` branch) aims to bring strict engineering standards, ultra-clean folder structures, and optimized mechanics. We are operating under Ponytail Ultra philosophy: No unrequested abstractions, no avoidable dependencies, minimum that works but incredibly clean and structured.

## Current Milestone: v8.0 Playable Scenes & Prototype Integration

**Goal:** Consolidate playable scenes into a functional flow, implement a scene-selection dummy menu, and port over previous work from the OGEM NAOR_prototype.

**Target features:**
- Fix and stabilize all playable scenes in the repository.
- Create a dummy scene-selection menu to transition properly into the playable areas via a "Start" action.
- Pull and integrate the relevant work from the `OTK-LAB/NAOR_prototype` repository.

## Current State
- **Shipped:** v4.0 Autonomous AI Art Pipeline (Completed: 2026-07-10).
- The pipeline orchestrator and C# ingestion scripts have been successfully implemented and verified with mock PNG generation. AI models are deferred to user download.

## Goals
1. Establish a pristine, strictly categorized directory structure (especially for `Assets`).
2. Perform a deep refactoring and optimization of the existing codebase.
3. Design and integrate a highly functional, better-looking main menu.
4. Merge existing scenes and refine the core gameplay mechanics and story flow.

## Technology Stack
- Unity (2D)
- C#
- Python (AI Art Pipeline: MoMask, ComfyUI, Blender on MPS)

## Team & Roles
- Efe Basol: Lead Developer & Architect

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd:complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-07-12 entering v8.0 milestone*
