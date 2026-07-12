using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using NAOR.Gameplay;

public class Phase83Setup : Editor
{
    [MenuItem("Tools/Setup Phase 8.3 (Return Hotkeys)")]
    public static void SetupPhase83()
    {
        // Save current scene to avoid losing work
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("Setup cancelled by user.");
            return;
        }

        string[] prototypeScenes = new string[]
        {
            "Assets/Scenes/Prototype/RiverScene.unity",
            "Assets/Scenes/Prototype/MainScene.unity",
            "Assets/Scenes/Prototype/Collesium.unity",
            "Assets/Scenes/Prototype/MiniBoss.unity",
            "Assets/Scenes/Prototype/Demo Final.unity",
            "Assets/Scenes/Prototype/AfterRiver.unity",
            "Assets/Scenes/Prototype/BeforeRiver.unity",
            "Assets/Scenes/Prototype/Bridge.unity"
        };

        foreach (string scenePath in prototypeScenes)
        {
            try
            {
                // Open the scene
                var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                // Check if EscapeToMenu already exists
                EscapeToMenu existingHook = GameObject.FindObjectOfType<EscapeToMenu>();
                
                if (existingHook == null)
                {
                    // Create hook object
                    GameObject hookObj = new GameObject("EscapeToMenuHook");
                    hookObj.AddComponent<EscapeToMenu>();
                    
                    // Mark scene as dirty and save
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene, scenePath);
                    Debug.Log($"[Phase 8.3] Added EscapeToMenuHook to {scene.name}");
                }
                else
                {
                    Debug.Log($"[Phase 8.3] EscapeToMenuHook already exists in {scene.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[Phase 8.3] Failed to process scene {scenePath}: {e.Message}");
            }
        }

        // Return to DummyMainMenu
        string dummyMenuPath = "Assets/Scenes/DummyMainMenu.unity";
        if (System.IO.File.Exists(dummyMenuPath))
        {
            EditorSceneManager.OpenScene(dummyMenuPath, OpenSceneMode.Single);
        }

        Debug.Log("Phase 8.3 Setup Complete! Return hotkeys have been injected into prototype scenes.");
    }
}
