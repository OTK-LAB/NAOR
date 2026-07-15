# Project Roadmap

## Milestone v8.0: Playable Scenes & Prototype Integration

### Phase 8.1: Prototype Scene Consolidation

**Focus:** Porting and stabilizing prototype scenes with modular initialization.
**Requirements:** CORE-03, CORE-05, CORE-06

**Success Criteria:**

- User can observe prototype assets and mechanics cleanly ported without legacy bloat.
- User can play consolidated, functional scenes ported from the prototype.
- User can play any ported scene individually in the Unity Editor without relying on a main menu entry point.

### Phase 8.2: Dummy Scene Selection Menu

**Focus:** Establishing the main entry point to the application.
**Requirements:** CORE-01

**Success Criteria:**

- User can open the application and arrive at a newly created dummy main menu scene.
- User can view a clear UI listing available prototype scenes to select.

### Phase 8.3: Native Scene Integration

**Focus:** Implementing clean scene transitions utilizing native Unity tools.
**Requirements:** CORE-02, CORE-04

**Success Criteria:**

- User can select a scene from the dummy menu and successfully transition into gameplay.
- User can transition between scenes without encountering monolithic wrapper scripts, using native `UnityEngine.SceneManagement`.
- User can transition smoothly between scenes without persistent state bleed.

### Phase 9: AI Pipeline v2.0 Upgrades

**Goal:** Raise sprite output quality and character consistency — curated per-character IPAdapter references (palette drift), improved Blender proxy anatomy, shading smoothness, and AutoSpriteImporter API modernization
**Requirements**: TBD
**Depends on:** Phase 8
**Plans:** 1/1 plans complete

Plans:

- [ ] 09-01-PLAN.md

- [x] TBD (run /gsd-plan-phase 9 to break down) (completed 2026-07-13)
