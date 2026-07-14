# Phase 8.3 Research: Native Scene Integration

## Objective
Establish a clean mechanism for transitioning into and out of gameplay scenes using native `UnityEngine.SceneManagement` without monolithic wrappers, satisfying CORE-02 and CORE-04, and preventing persistent state bleed.

## Context & Findings
1. **Current Scene Transitions**:
   - The Dummy Main Menu uses a simple `SceneManager.LoadScene()` call.
   - Various prototype scenes contain scattered legacy scripts for scene transitioning (e.g., `SceneTransition.cs`, `SceneChanger.cs`, `NextScene.cs`, `PrologueManager.cs`). These scripts often have redundant or confusing wrappers, loading scenes by index or string unpredictably.
   - There are some persistent singletons in the project (`AudioManager`, `Systems`), but they use a pattern that destroys duplicates. However, relying on them for core loading transitions violates the "Ponytail Ultra" architecture principle of avoiding monolithic wrappers.

2. **Transitioning Out of Gameplay**:
   - Currently, if a user enters a prototype scene from the Dummy Main Menu, they might be stuck unless that specific prototype scene has a UI to go back.
   - To truly verify smooth transitions and lack of state bleed, the user needs to be able to jump from Menu -> Scene A -> Menu -> Scene B, or Menu -> Scene A -> Menu -> Scene A again to see if things duplicate or break.

## Proposed Approach
1. **Create a Universal "Return to Menu" Mechanism**:
   - Implement a lightweight, non-persistent script `ReturnToMenu.cs` or an input listener that triggers `SceneManager.LoadScene("DummyMainMenu")` when the Escape key (or a UI button) is pressed.
   - This script can be added to a prefab and dropped into prototype scenes, or we can instruct the user to ensure it's available.
   - Alternatively, since Phase 8.1 consolidated scenes, we just need to ensure the existing end-of-level triggers (like doors) use `SceneManager.LoadScene("DummyMainMenu")` or the next scene directly, rather than routing through a heavy `GameManager`.

2. **Refactor Legacy Loaders**:
   - In a strict phase, we would delete `SceneChanger.cs` and replace it with a simple native call. For this plan, we will create a clean, standardized native component `NativeSceneLoader.cs` that can be dropped onto trigger zones (like Doors) to load a target scene string. This fulfills the "no monolithic wrappers" requirement.

## Conclusion
The plan will involve creating a standard, modular `NativeSceneLoader` for going forward, and a `ReturnToMenu` input hook for going backward, all relying purely on `UnityEngine.SceneManagement.SceneManager` without any global state or `DontDestroyOnLoad` managers.
