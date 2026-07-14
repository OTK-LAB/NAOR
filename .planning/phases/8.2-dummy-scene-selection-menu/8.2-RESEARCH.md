# Phase 8.2 Research: Dummy Scene Selection Menu

## Objective
Research the necessary components for establishing a dummy main menu scene as the primary entry point to the application, fulfilling requirement CORE-01.

## Findings
1. **Existing Scenes**:
   - There are several prototype scenes under `Assets/Scenes/Prototype/`:
     - `RiverScene.unity`
     - `MainScene.unity`
     - `Collesium.unity`
     - `MiniBoss.unity`
     - `Demo Final.unity`
     - `AfterRiver.unity`
     - `BeforeRiver.unity`
     - `Bridge.unity`
   - There is an existing `Assets/Scenes/MainMenuScene.unity` but phase 8.2 specifically asks for a "newly created dummy main menu scene". We will create `Assets/Scenes/DummyMainMenu.unity` to serve this purpose without conflicting with any existing complex main menu.

2. **Requirements**:
   - The scene must list available prototype scenes.
   - The UI needs to be clear and selectable.
   - The transitions will use native `UnityEngine.SceneManagement` (this is also mentioned in CORE-04 for Phase 8.3, but the basic loading must work here).

3. **Implementation Plan Approach**:
   - Create a new scene `Assets/Scenes/DummyMainMenu.unity`.
   - Build a Canvas with a ScrollView or a simple VerticalLayoutGroup of Buttons.
   - Create a minimal script `SceneSelectionUI.cs` to handle button clicks and load the respective scene.
   - Add all relevant scenes (DummyMainMenu at index 0, plus prototype scenes) to the Unity Editor Build Settings via a script or instruct the user to do so, though typically in automated plans we might provide an editor script to do this or assume the execution phase will handle it.

## Conclusion
The requirements are straightforward. We need to create a UI-driven dummy menu scene and a simple script to trigger `SceneManager.LoadScene()`.
