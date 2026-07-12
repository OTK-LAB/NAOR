# Phase 8.3 Plan: Native Scene Integration

## Context
**Objective:** Implement clean scene transitions utilizing native Unity tools, and establish a mechanism to safely return to the main menu without state bleed.
**Requirements:** CORE-02, CORE-04
**Success Criteria:**
- User can select a scene from the dummy menu and successfully transition into gameplay. (Covered via Phase 8.2 execution, verified here).
- User can transition between scenes without encountering monolithic wrapper scripts, using native `UnityEngine.SceneManagement`.
- User can transition smoothly between scenes without persistent state bleed (e.g., repeatedly entering and exiting scenes).

## Execution Steps

### 1. Create a Universal Native Scene Loader
- **File:** `Assets/Scripts/Gameplay/NativeSceneLoader.cs`
- **Action:** Create a C# script to replace legacy `SceneChanger` or `PrologueManager` logic.
- **Details:** 
  - Add `using UnityEngine.SceneManagement;`
  - Create a public method `public void LoadScene(string sceneName)` that calls `SceneManager.LoadScene(sceneName)`.
  - Create a public method `public void ReturnToMainMenu()` that calls `SceneManager.LoadScene("DummyMainMenu")`.
  - Provide an optional `public string triggerSceneName;` and an `OnTriggerEnter2D(Collider2D col)` / `OnTriggerEnter(Collider col)` logic to auto-load a scene when a player walks into a door/zone.

### 2. Implement Hotkey for Returning to Menu
- **File:** `Assets/Scripts/Gameplay/EscapeToMenu.cs`
- **Action:** Create a lightweight utility script that can be dropped into any prototype scene.
- **Details:**
  - In `Update()`, check `Input.GetKeyDown(KeyCode.Escape)`.
  - If pressed, call `SceneManager.LoadScene("DummyMainMenu")`.
  - This allows developers and players to back out of a scene to verify that state unloads cleanly, without needing to hook up a full Pause UI for this MVP milestone.

### 3. Editor Utility for Quick Setup
- **File:** `Assets/Editor/Phase83Setup.cs`
- **Action:** Create a menu item `Tools > Setup Phase 8.3 (Return Hotkeys)`.
- **Details:**
  - This script iterates through all prototype scenes in the Build Settings.
  - It opens each scene, checks if an `EscapeToMenu` object exists.
  - If not, it creates a new empty GameObject `EscapeToMenuHook`, attaches the `EscapeToMenu` script, marks it as EditorOnly/Scene utility, and saves the scene.
  - This guarantees that every prototype scene can transition back smoothly without manual labor.

## Verification
- Enter `DummyMainMenu` in play mode.
- Click a prototype scene button (e.g., `RiverScene`).
- Inside the scene, press `Escape` to return to the Main Menu.
- Enter the same scene again. Ensure there are no duplicate players, broken audio, or missing references (verifying no state bleed).
- Walk into a transition door (if hooked up with `NativeSceneLoader`) and ensure it loads cleanly without error.
