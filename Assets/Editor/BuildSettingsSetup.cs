using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility to configure Build Settings for Phase 8.2.
/// Adds DummyMainMenu at index 0 followed by all prototype scenes.
/// Usage: Tools > Setup Build Scenes for Phase 8.2
/// </summary>
public static class BuildSettingsSetup
{
    private const string DummyMainMenuPath = "Assets/Scenes/DummyMainMenu.unity";

    private static readonly string[] PrototypeScenePaths = new[]
    {
        "Assets/Scenes/Prototype/RiverScene.unity",
        "Assets/Scenes/Prototype/MainScene.unity",
        "Assets/Scenes/Prototype/Collesium.unity",
        "Assets/Scenes/Prototype/MiniBoss.unity",
        "Assets/Scenes/Prototype/Demo Final.unity",
        "Assets/Scenes/Prototype/AfterRiver.unity",
        "Assets/Scenes/Prototype/BeforeRiver.unity",
        "Assets/Scenes/Prototype/Bridge.unity",
    };

    [MenuItem("Tools/Setup Build Scenes for Phase 8.2")]
    public static void SetupBuildScenes()
    {
        var scenes = new List<EditorBuildSettingsScene>();

        // DummyMainMenu at index 0 (entry point)
        scenes.Add(new EditorBuildSettingsScene(DummyMainMenuPath, true));

        // All prototype scenes in the declared order
        foreach (string path in PrototypeScenePaths)
        {
            scenes.Add(new EditorBuildSettingsScene(path, true));
        }

        EditorBuildSettings.scenes = scenes.ToArray();

        Debug.Log(
            $"[BuildSettingsSetup] Build Settings updated: {scenes.Count} scene(s) registered. " +
            $"DummyMainMenu is at index 0."
        );
    }
}
