using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NAOR.UI;
using System.Collections.Generic;

public class Phase82Setup : Editor
{
    [MenuItem("Tools/Setup Phase 8.2 (Dummy Main Menu)")]
    public static void SetupPhase82()
    {
        string scenePath = "Assets/Scenes/DummyMainMenu.unity";

        // Create new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Add Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        cameraObj.tag = "MainCamera";

        // Add EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        eventSystemObj.AddComponent<StandaloneInputModule>();

        // Add Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Add UI Manager
        SceneSelectionUI uiScript = canvasObj.AddComponent<SceneSelectionUI>();

        // Title Text
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(canvasObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "Prototype Scene Selection";
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 48;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -50);
        titleRect.sizeDelta = new Vector2(600, 100);

        // Scroll View / Panel for Buttons
        GameObject panelObj = new GameObject("ButtonPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        panelObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0, -50);
        panelRect.sizeDelta = new Vector2(400, 600);

        VerticalLayoutGroup vlg = panelObj.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlHeight = false;
        vlg.childControlWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.spacing = 15;
        vlg.padding = new RectOffset(20, 20, 20, 20);

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

        foreach (string protoScene in prototypeScenes)
        {
            CreateSceneButton(protoScene, panelObj.transform, uiScript);
        }

        // Save Scene
        EditorSceneManager.SaveScene(newScene, scenePath);

        // Update Build Settings
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        
        // Add DummyMainMenu first
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));

        // Add prototype scenes
        foreach (string protoScene in prototypeScenes)
        {
            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(protoScene, true));
        }

        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

        Debug.Log("Phase 8.2 Setup Complete! Scene created and Build Settings updated.");
    }

    private static void CreateSceneButton(string scenePath, Transform parent, SceneSelectionUI uiScript)
    {
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        GameObject btnObj = new GameObject($"Button_{sceneName}");
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f);
        
        Button btn = btnObj.AddComponent<Button>();
        
        SceneButtonHelper helper = btnObj.AddComponent<SceneButtonHelper>();
        helper.sceneName = sceneName;
        helper.uiScript = uiScript;

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(300, 50);

        GameObject txtObj = new GameObject("Text");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.text = sceneName;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize = 24;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        txtRect.offsetMin = Vector2.zero;
        txtRect.offsetMax = Vector2.zero;
    }
}
