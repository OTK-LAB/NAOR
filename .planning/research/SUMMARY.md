# Project Research Summary

**Project:** NAOR
**Domain:** Unity 2D Scene Management & UI
**Researched:** 2026-07-12
**Confidence:** HIGH

## Executive Summary

This project involves implementing a minimal Unity 2D Scene Management and UI system to integrate playable scenes from a prototype into a clean, cohesive project structure. Based on research and established practices, the overarching strategy prioritizes the "Ponytail Ultra" philosophy—leveraging native Unity tools instead of third-party frameworks, and prioritizing explicit scene-scoped references over monolithic singletons.

The recommended approach relies directly on `UnityEngine.SceneManagement` and basic `UnityEngine.UI` (UGUI) to deliver a dummy scene-selection menu. This avoids technical debt and ensures fast transition flows without abstraction bloat. Key risks center around scene transition state bleed, unintentional import of prototype bloat, and broken editor testing workflows. Mitigating these involves strict lifecycle cleanup, surgical extraction of prototype assets into isolated folders, and localized fallback bootstrap scripts for editor play-mode testing.

## Key Findings

### Recommended Stack

The research emphasizes using native, built-in features to maintain minimal overhead and high performance.

**Core technologies:**
- `UnityEngine.SceneManagement`: Scene loading and transitions — Standard module, zero overhead, perfectly handles basic changes.
- `UnityEngine.UI` (UGUI): Dummy Scene-Selection Menu — Lightweight and built-in; avoids UI Toolkit overkill for a simple prototype menu.
- `System.Collections` (Coroutines): Asynchronous Loading — Enables `LoadSceneAsync` to avoid freezing the main thread if loading larger scenes.

### Expected Features

Features are strictly prioritized according to MVP validation needs.

**Must have (table stakes):**
- Dummy Main Menu — Provides a clear entry point to the application.
- Scene Transition Logic — Core mechanism to move from menu to gameplay without hanging.
- Playable Scene Consolidation — Clean extraction of at least one functional scene from the prototype.

**Should have (competitive):**
- Ponytail Ultra Architecture — Extremely clean, zero-abstraction scene loading.
- Modular Scene Initialization — Scenes don't rely on hidden global states; testable in isolation.

**Defer (v2+):**
- Multiple Scene Selection
- Loading Screen UI
- Save/Load System
- Full Settings/Pause Menu

### Architecture Approach

The architecture favors direct, lightweight scene loading without intermediary management wrappers. 

**Major components:**
1. `MainMenuController` — Handles root menu interactions and toggling the scene selection overlay.
2. `SceneSelectionController` — Manages the dummy scene-selection grid/list UI logic.
3. Unity `SceneManager` — Handles the native transition between scenes directly based on selection.

### Critical Pitfalls

1. **Scene Transition State Bleed (Singleton Abuse)** — Avoid global `DontDestroyOnLoad` singletons. Implement explicit initialization and teardown sequences per scene.
2. **Prototype Bloat Import** — Do not drag-and-drop entire legacy folders. Surgically copy only essential raw assets and rewrite dependencies.
3. **Broken Editor Workflow (Missing Entry Point)** — Add minimal fallback bootstrap scripts (`#if UNITY_EDITOR`) in playable scenes so they can be tested directly without the main menu.
4. **Hardcoded Scene Loading (Magic Strings)** — Use a central static registry or Enums for scene references instead of raw string literals.

## Implications for Roadmap

Based on research, suggested phase structure:

### Phase 1: Prototype Scene Consolidation
**Rationale:** A pristine directory structure is a prerequisite for clean scene transitions. Unstable prototypes must be stabilized before integration.
**Delivers:** Working imported scenes isolated in `Assets/Scenes/Prototype/`.
**Addresses:** Playable Scene Consolidation, Prototype Work Porting.
**Avoids:** Prototype Bloat Import pitfall.

### Phase 2: Dummy Scene Selection Menu
**Rationale:** The menu requires the target scenes to be stable and available in the Build Settings.
**Delivers:** A lightweight UI Canvas panel to choose prototypes.
**Uses:** `UnityEngine.UI` (UGUI), `SceneSelectionController`.
**Implements:** The presentation layer of the `MainMenuScene`.

### Phase 3: Native Scene Integration
**Rationale:** Final hookup connecting the presentation layer (menu) to the data layer (scenes).
**Delivers:** Complete functional transition loop from Menu to Game and back.
**Addresses:** Scene Transition Logic, Dummy Main Menu flow.
**Avoids:** Scene Transition State Bleed, Hardcoded Scene Loading.

### Phase Ordering Rationale

- **Asset cleanliness first:** Importing external prototypes directly into the main project creates immediate tech debt. Phase 1 forces surgical extraction.
- **Top-down menu creation:** Phase 2 implements the menu layer cleanly without worrying about transitions yet.
- **Flow completion:** Phase 3 merges them together using the safest native `SceneManager` calls, verifying that state lifecycle rules are obeyed.

### Research Flags

Phases likely needing deeper research during planning:
- **Phase 1:** Requires targeted code review of `NAOR_prototype` components to ensure no bloated or deprecated systems are ported over. 

Phases with standard patterns (skip research-phase):
- **Phase 2 & 3:** Standard UGUI and native Unity `SceneManager` practices apply.

## Confidence Assessment

| Area | Confidence | Notes |
|------|------------|-------|
| Stack | HIGH | Based purely on stable, built-in Unity components. |
| Features | HIGH | Highly scoped and targeted toward minimum viable prototype. |
| Architecture | HIGH | Simple, flat structure conforming strictly to internal standards. |
| Pitfalls | HIGH | Common Unity 2D architectural pitfalls are well-documented. |

**Overall confidence:** HIGH

### Gaps to Address

- **Prototype Dependencies:** Need to evaluate what explicit dependencies the old `NAOR_prototype` mechanics rely on so they can be stripped down or recreated appropriately.

## Sources

### Primary (HIGH confidence)
- Unity Official Documentation — Scene Management and UGUI best practices.
- `.planning/PROJECT.md` — Project guidelines, driving the "Ponytail Ultra" philosophy.
- Context7 — Validated current 2026 structural patterns for Unity.

### Secondary (MEDIUM confidence)
- Internal Domain Expertise — Common Unity 2D Development Gotchas.

---
*Research completed: 2026-07-12*
*Ready for roadmap: yes*
