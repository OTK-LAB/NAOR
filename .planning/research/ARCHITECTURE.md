# Architecture Research

**Domain:** Scene Management & UI
**Researched:** 2026-07-12
**Confidence:** HIGH

## Standard Architecture

### System Overview

```
┌─────────────────────────────────────────────────────────────┐
│                       Presentation Layer                     │
├─────────────────────────────────────────────────────────────┤
│  ┌────────────────┐  ┌────────────────┐                     │
│  │    MainMenu    │  │ SceneSelection │                     │
│  │   Controller   │  │   Controller   │                     │
│  └───────┬────────┘  └───────┬────────┘                     │
│          │                   │                              │
├──────────┴───────────────────┴──────────────────────────────┤
│                         System Layer                         │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────────────────────────────────────────────┐    │
│  │              Unity SceneManager (Native)            │    │
│  └───────────────────────────┬─────────────────────────┘    │
├──────────────────────────────┴──────────────────────────────┤
│                         Data Layer                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │   Scene 1    │  │   Scene 2    │  │   Scene N    │       │
│  │ (Prototype)  │  │ (Prototype)  │  │ (Prototype)  │       │
│  └──────────────┘  └──────────────┘  └──────────────┘       │
└─────────────────────────────────────────────────────────────┘
```

### Component Responsibilities

| Component | Responsibility | Typical Implementation |
|-----------|----------------|------------------------|
| `MainMenuController` | Handles root menu interactions (Start, Options, Quit). | Existing `MonoBehaviour` bound to UI buttons. Needs modification to open Scene Selection instead of direct load. |
| `SceneSelectionController` | Manages the dummy scene-selection menu overlay. | New `MonoBehaviour` managing a grid/list of UI buttons, each loading a specific prototype scene. |
| Unity `SceneManager` | Handles transitioning between scenes. | Native Unity API. No need for complex async loaders yet per Ponytail philosophy. |

## Recommended Project Structure

```
Assets/
├── Scenes/
│   ├── MainMenuScene.unity         # Existing
│   └── Prototype/                  # NEW: Imported scenes from NAOR_prototype
│       ├── Level_01.unity
│       └── Level_02.unity
├── Scripts/
│   └── UI/
│       ├── MainMenuController.cs   # Modified
│       └── Menu/
│           └── SceneSelectionController.cs # NEW
└── Prefabs/
    └── UI/
        └── SceneSelectionPanel.prefab # NEW
```

### Structure Rationale

- **`Scenes/Prototype/`:** Isolates the imported playable scenes from the existing clean structure, preventing asset pollution while they are being integrated and evaluated.
- **`Scripts/UI/Menu/`:** Keeps the new selection controller grouped with existing menu-related scripts.
- **`Prefabs/UI/`:** Stores the scene selection panel as a prefab so it can be cleanly instantiated or referenced.

## Architectural Patterns

### Pattern 1: Direct Scene Invocation (Ponytail Ultra)

**What:** Direct calls to `SceneManager.LoadScene(string name)` triggered by UI buttons.
**When to use:** When scene transitions are lightweight, and a heavy loading screen or async operation is not strictly required.
**Trade-offs:** 
- *Pros:* Zero overhead, perfectly aligned with minimum-that-works philosophy, no extra boilerplate.
- *Cons:* UI freezes briefly while scene loads; not scalable for massive open-world scenes.

**Example:**
```csharp
public void OnSceneSelected(string sceneName)
{
    SceneManager.LoadScene(sceneName);
}
```

## Data Flow

### Request Flow

```
[User clicks Start]
    ↓
[MainMenuController] → Activates → [SceneSelectionPanel]
    ↓
[User clicks "Level 1"]
    ↓
[SceneSelectionController] → Calls → [SceneManager.LoadScene("Level_01")]
    ↓
[Level_01 Scene Active]
```

## Scaling Considerations

| Scale | Architecture Adjustments |
|-------|--------------------------|
| Small Prototype | Direct `SceneManager.LoadScene` is perfect. |
| Beta / Heavy Assets | Introduce a simple `AsyncSceneLoader` with a fade-in/fade-out UI canvas. |
| Full Game | Addressable Asset System for scene loading to manage memory. |

### Scaling Priorities

1. **First bottleneck:** Load times freeze the main thread. Fix by moving to `SceneManager.LoadSceneAsync` with a bare-bones loading screen.
2. **Second bottleneck:** Prototype scenes have missing references or missing global persistent managers. Fix by adding a lightweight `GameManager` or initialization scene if required.

## Anti-Patterns

### Anti-Pattern 1: Over-engineered Scene Managers

**What people do:** Build massive Singleton `SceneManagerWrapper` classes that handle async loading, progress bars, state saving, and event broadcasting just to load a test scene.
**Why it's wrong:** Violates Ponytail guidelines. It adds unnecessary complexity and debugging overhead for a simple prototype milestone.
**Do this instead:** Stick to Unity's native `SceneManager` direct calls until the frame drops or load times necessitate an async loader.

## Integration Points

### Internal Boundaries

| Boundary | Communication | Notes |
|----------|---------------|-------|
| `MainMenu` ↔ `SceneSelection` | Direct reference (UI activation) | `MainMenuController` will toggle the visibility of the selection panel. |
| `SceneSelection` ↔ `Prototype Scenes` | Native `SceneManager` API | Selection UI simply triggers native load. Ensure imported scenes are added to the Build Settings. |

### Suggested Build Order

1. **Isolate Import:** Pull relevant playable scenes from `OTK-LAB/NAOR_prototype` and place them strictly in `Assets/Scenes/Prototype/` and their respective assets in `Assets/Art/Prototype/` to maintain directory cleanliness.
2. **Add to Build Settings:** Ensure all imported prototype scenes are added to Unity's Build Settings so they can be loaded.
3. **UI Implementation:** Create the dummy `SceneSelectionPanel` UI within the `MainMenuScene`.
4. **Controller Logic:** Implement `SceneSelectionController.cs` to map UI buttons to the newly imported scene names.
5. **Flow Integration:** Modify `MainMenuController.PlayGame()` to show the selection panel instead of hard-loading `CombatImprovementTest`.
