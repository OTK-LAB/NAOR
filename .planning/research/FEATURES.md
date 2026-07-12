# Feature Research

**Domain:** Scene Management & Dummy Menu (Game Prototype)
**Researched:** 2026-07-12
**Confidence:** HIGH

## Feature Landscape

### Table Stakes (Users Expect These)

Features users assume exist. Missing these = product feels incomplete.

| Feature | Why Expected | Complexity | Notes |
|---------|--------------|------------|-------|
| **Dummy Main Menu** | Provides a clear entry point to the application. | LOW | Needs basic UI Canvas, Button, and simple script. |
| **Scene Transition Logic** | Clicking "Start" should load the game. | LOW | Use Unity's `SceneManager` asynchronously to avoid hanging. |
| **Playable Scene Consolidation** | Core mechanics must function correctly when loaded. | MEDIUM | Depends on resolving asset references and clean directory structure. |

### Differentiators (Competitive Advantage)

Features that set the product apart. Not required, but valuable.

| Feature | Value Proposition | Complexity | Notes |
|---------|-------------------|------------|-------|
| **Ponytail Ultra Architecture** | Extremely clean, zero-abstraction scene loading. Fast loading, low bug rate. | MEDIUM | Needs strict enforcement of the no-abstraction rule from `PROJECT.md`. |
| **Modular Scene Initialization** | Scenes don't rely on hidden global states; testable in isolation. | MEDIUM | Requires clear entry-point scripts without spaghetti coupling. |

### Anti-Features (Commonly Requested, Often Problematic)

Features that seem good but create problems.

| Feature | Why Requested | Why Problematic | Alternative |
|---------|---------------|-----------------|-------------|
| **Complex Transition Animations** | Looks professional (fades, wipes). | Unnecessary overhead for a prototype, violates "minimum that works". | Instant cut or a simple, standard black screen. |
| **Global Singletons for Everything** | Easy to access data across scenes. | Creates rigid coupling and technical debt. | Dependency injection, explicit passing, or ScriptableObjects. |
| **Full Settings Menu** | Games typically need settings. | Distraction from MVP playable scene goals. | Hardcoded defaults or simple debug toggles. |

## Feature Dependencies

```
[Playable Scene Consolidation]
    └──requires──> [Asset Structure Categorization]

[Dummy Main Menu]
    └──requires──> [Scene Transition Logic]
                       └──requires──> [Playable Scene Consolidation]

[Prototype Work Porting (NAOR_prototype)]
    └──enhances──> [Playable Scene Consolidation]
```

### Dependency Notes

- **Playable Scene Consolidation requires Asset Structure Categorization:** A pristine directory structure is a prerequisite (as per `PROJECT.md` goals) to integrate scenes cleanly.
- **Scene Transition Logic requires Playable Scene Consolidation:** Cannot transition to scenes that aren't integrated and stable.
- **Dummy Main Menu requires Scene Transition Logic:** The menu's primary function is to trigger these transitions.
- **Prototype Work Porting enhances Playable Scene Consolidation:** Brings in the actual mechanics to make the scenes truly "playable."

## MVP Definition

### Launch With (v1 - Milestone v8.0)

Minimum viable product — what's needed to validate the concept.

- [x] **Dummy Main Menu** — Provides the necessary entry point to validate scene flow.
- [x] **Scene Transition Logic** — Core mechanism to move from menu to gameplay.
- [x] **Playable Scene Consolidation** — At least one fully functional scene ported from the prototype, stabilized and clean.

### Add After Validation (v1.x)

Features to add once core is working.

- [ ] **Multiple Scene Selection** — Choosing specific levels/prototypes from the menu.
- [ ] **Loading Screen UI** — If scenes become heavy enough to cause noticeable hangs during asynchronous loading.

### Future Consideration (v2+)

Features to defer until product-market fit is established.

- [ ] **Save/Load System** — To persist state across scenes.
- [ ] **Full Settings/Pause Menu** — Audio, video, and control configurations.

## Feature Prioritization Matrix

| Feature | User Value | Implementation Cost | Priority |
|---------|------------|---------------------|----------|
| Dummy Main Menu | HIGH | LOW | P1 |
| Scene Transition Logic | HIGH | LOW | P1 |
| Playable Scene Consolidation | HIGH | MEDIUM | P1 |
| Prototype Porting | HIGH | HIGH | P1 |
| Transition Animations | LOW | LOW | P3 |
| Save/Load System | MEDIUM | HIGH | P3 |

**Priority key:**
- P1: Must have for launch
- P2: Should have, add when possible
- P3: Nice to have, future consideration

## Competitor Feature Analysis

| Feature | Typical Game Prototype | Our Approach (Ponytail Ultra) |
|---------|------------------------|-------------------------------|
| **Scene Loading** | Heavy GameManager singletons. | Direct, clean `SceneManager` calls with minimal state transfer. |
| **Menu Architecture** | Deeply nested UI prefabs. | Flat, minimal Canvas with only necessary prototype buttons. |

## Sources

- `.planning/PROJECT.md` (v8.0 Playable Scenes & Prototype Integration)
- Ponytail Ultra Philosophy (Strict engineering, no avoidable dependencies)
- Standard Unity Architecture Guidelines

---
*Feature research for: Playable scenes and dummy menu integration*
*Researched: 2026-07-12*
