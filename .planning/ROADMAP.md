# NAOR - Roadmap

## Milestones

- ✅ **v4.0 Autonomous AI Art Pipeline** — Phase 4.1 (shipped 2026-07-10)
- 🚧 **v1.0 Refactor & Optimization** — Phases 1.1-1.2 (in progress)

## Phases

<details>
<summary>✅ v4.0 Autonomous AI Art Pipeline (Phase 4.1) — SHIPPED 2026-07-10</summary>

- [x] Phase 4.1: End-to-End ComfyUI & MoMask Pipeline Integration — completed 2026-07-10

</details>

### 🚧 v1.0 Refactor & Optimization (In Progress)

- [x] Phase 1.1: Directory Structure (0 plans) (completed 2026-07-09)
- [ ] Phase 1.2: Codebase Audit (0 plans)

### Phase 1.1: Directory Structure

**Goal:** Clean up directory structure and assets
**Success criteria:**

1. All assets are categorized into Art, Scripts, and Prefabs.
2. No loose scripts or prefabs remain in the root of the Assets folder.

### Phase 1.2: Codebase Audit

**Goal:** Remove dead code and optimize performance
**Success criteria:**

1. Unnecessary abstractions are removed from the codebase.
2. Code executes faster and follows Ponytail Ultra principles.

## Progress

| Phase | Milestone | Plans Complete | Status | Completed |
|-------|-----------|----------------|--------|-----------|
| 1.1. Directory Structure | v1.0 | 1/1 | Complete    | 2026-07-09 |
| 1.2. Codebase Audit | v1.0 | 0/0 | Not started | - |

## Backlog / Planned

### 🚧 v2.0 UI Overhaul

**Goal**: Create a highly functional, robust Menu system.

- [ ] Phase 2.1: Design and implement the new Main Menu.
- [ ] Phase 2.2: Connect the menu to the game state and settings.

### 🚧 v3.0 Scenes, Story, and Mechanics

**Goal**: Unify scenes and finalize core mechanics/story.

- [ ] Phase 3.1: Merge the two main scenes.
- [ ] Phase 3.2: Mechanics review and new mechanics integration.
- [ ] Phase 3.3: Story writing and development integration.

## Milestone 5: AI Model Installation — ✅ COMPLETED 2026-07-10
**Goal**: Download and configure the heavy AI models (ComfyUI, SDXL, MoMask) on the external SSD so the autonomous pipeline becomes fully functional.
- [x] Phase 5.1: Install Blender (headless) and ComfyUI Base
- [x] Phase 5.2: Download and Configure ControlNet/SDXL Models
- [x] Phase 5.3: Setup PyTorch for MoMask and Download Tensors
- [x] Phase 5.4: Download BiRefNet Models (Gap Closure)

## Milestone 6: Pipeline Reality — 🚧 IN PROGRESS (started 2026-07-11)
**Goal**: Replace every mocked module of the AI art pipeline (v4.0 shipped orchestration skeleton only — all 5 stages were stubs) with real, verified implementations producing actual sprite animations.

**Audit note (2026-07-11):** v4.0 "end-to-end verification" only validated file plumbing. `text_to_motion.py`, `blender_render.py`, `comfy_client.py`, `pack_sprites.py` all contained `time.sleep` mocks writing text placeholders; `workflows/` was empty. Milestone 6 closes this gap.

- [ ] Phase 6.1: Real MoMask text→BVH generation (MPS/CPU, venv compatibility fix if needed)
- [ ] Phase 6.2: Real Blender headless render — BVH retarget to simple humanoid, beauty + depth frames
- [ ] Phase 6.3: Real ComfyUI workflow (SDXL + ControlNet Depth) + API client + background removal
- [ ] Phase 6.4: Real sprite packing, end-to-end run, Unity ingestion check

**Known risks:** MoMask deps on Python 3.14; no rigged mesh existed before 6.2; AnimateDiff absent (temporal consistency deferred — per-frame fixed-seed first); t2i-adapter openpose is not a ControlNet; style/character consistency needs LoRA/IPAdapter later.
