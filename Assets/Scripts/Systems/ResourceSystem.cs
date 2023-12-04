using System;
using UnityEditor;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    private static ResourceSystem _i;

    public static ResourceSystem I
    {
        get
        {
            if (_i == null)
            {
                _i = (Instantiate(Resources.Load("ResourceSystem")) as GameObject).GetComponent<ResourceSystem>();
                _i.gameObject.name = "ResourceSystem";
            }
            return _i;
        }
    }

    #region Resource Classes
    [System.Serializable]
    public class AudioClips<T>
    {
        public T clip;
        public AudioClip audio;
    }
    [System.Serializable]
    public class Audio
    {
        public AudioClips<AudioSystem.Audio.SFX>[] SFX;
        public AudioClips<AudioSystem.Audio.VoiceLine>[] VoiceLine;
        public AudioClips<AudioSystem.Audio.Soundtrack>[] Soundtrack;
    }

    [System.Serializable]
    public class VisualEffect<T>
    {
        public T name;
        public GameObject prefab;
    }
    [System.Serializable]
    public class VisualEffects
    {
        public VisualEffect<VFXSystem.VFX.ParticleSystem>[] Particle;
        public VisualEffect<VFXSystem.VFX.VFXGraph>[] VFXGraph;
    }

    [System.Serializable]
    public class PrefabWithName
    {
        public string name;
        public GameObject prefab;
    }
    [System.Serializable]
    public class PrefabsWithCategory
    {
        public string category;
        public PrefabWithName[] prefabs;
    }
    #endregion

    public Audio Audios;
    public VisualEffects Visuals;
    public PrefabsWithCategory[] Prefabs;

    /// <summary>
    /// Returns GameObject prefab of given category and name stored in Resource System, returns error if not any.
    /// </summary>
    public GameObject GetPrefab(string category, string name)
    {
        GameObject prefab;
        prefab = Array.Find(Array.Find(Prefabs, x => x.category == category).prefabs, x => x.name == name).prefab;
        if (prefab == null)
        {
            Debug.LogError($"Couldn't find match for a prefab stored at category: {category} and named: {name}");
        }
        return prefab;
    }
}

public class ResourceSystemEditor : EditorWindow
{
    ResourceSystem target;
    SerializedObject soTarget;

    SerializedProperty propAudios;
    SerializedProperty propVisuals;
    SerializedProperty propPrefabs;

    private Vector2 scrollPos;
    private int categoryIndexToEdit = -1;
    private bool isEditingCategory = false;

    [MenuItem("Window/Resource System")]
    public static void ShowWindow()
    {
        GetWindow<ResourceSystemEditor>("Resource System");
    }

    private void OnEnable()
    {
        var loadedResource = Resources.Load("ResourceSystem") as GameObject;
        target = loadedResource != null ? loadedResource.GetComponent<ResourceSystem>() : null;
        if (target != null)
        {
            soTarget = new SerializedObject(target);
            propAudios = soTarget.FindProperty("Audios");
            propVisuals = soTarget.FindProperty("Visuals");
            propPrefabs = soTarget.FindProperty("Prefabs");
        }
    }

    void OnGUI()
    {
        if (soTarget == null) return;

        soTarget.Update();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.PropertyField(propAudios);
        EditorGUILayout.PropertyField(propVisuals);

        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(3);
        propPrefabs.isExpanded = EditorGUILayout.Foldout(propPrefabs.isExpanded, "Prefabs");
        EditorGUILayout.EndHorizontal();
        if (propPrefabs.isExpanded)
        {
            EditorGUI.indentLevel++;

            int categoryIndexToDelete = -1;

            for (int i = 0; i < propPrefabs.arraySize; i++)
            {
                SerializedProperty categoryProp = propPrefabs.GetArrayElementAtIndex(i);

                SerializedProperty categoryNameProp = categoryProp.FindPropertyRelative("category");
                SerializedProperty prefabsProp = categoryProp.FindPropertyRelative("prefabs");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(3);
                categoryNameProp.isExpanded = EditorGUILayout.Foldout(categoryNameProp.isExpanded, categoryNameProp.stringValue);
                if (i == categoryIndexToEdit && isEditingCategory)
                {
                    categoryNameProp.stringValue = EditorGUILayout.TextField(categoryNameProp.stringValue);
                }
                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    if (categoryIndexToEdit == i)
                    {
                        isEditingCategory = !isEditingCategory;
                    }
                    else
                    {
                        categoryIndexToEdit = i;
                        isEditingCategory = true;
                    }
                }
                if (GUILayout.Button("Delete", GUILayout.Width(50)))
                {
                    categoryIndexToDelete = i;
                }
                EditorGUILayout.EndHorizontal();

                if (categoryNameProp.isExpanded)
                {
                    EditorGUI.indentLevel++;

                    int prefabIndexToDelete = -1;

                    for (int j = 0; j < prefabsProp.arraySize; j++)
                    {
                        SerializedProperty prefabProp = prefabsProp.GetArrayElementAtIndex(j);

                        SerializedProperty prefabNameProp = prefabProp.FindPropertyRelative("name");
                        SerializedProperty prefabObjectProp = prefabProp.FindPropertyRelative("prefab");

                        EditorGUILayout.BeginHorizontal();
                        prefabNameProp.stringValue = EditorGUILayout.TextField(prefabNameProp.stringValue);
                        prefabObjectProp.objectReferenceValue = EditorGUILayout.ObjectField(prefabObjectProp.objectReferenceValue, typeof(GameObject), false);
                        if (GUILayout.Button("Delete", GUILayout.Width(50)))
                        {
                            prefabIndexToDelete = j;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (prefabIndexToDelete != -1)
                    {
                        prefabsProp.DeleteArrayElementAtIndex(prefabIndexToDelete);
                    }

                    if (GUILayout.Button("Add Prefab"))
                    {
                        prefabsProp.arraySize++;
                    }

                    EditorGUI.indentLevel--;
                }
            }

            if (categoryIndexToDelete != -1)
            {
                propPrefabs.DeleteArrayElementAtIndex(categoryIndexToDelete);
                if (categoryIndexToDelete == categoryIndexToEdit)
                {
                    categoryIndexToEdit = -1;
                    isEditingCategory = false;
                }
            }

            if (GUILayout.Button("Add Category"))
            {
                propPrefabs.arraySize++;
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();

        soTarget.ApplyModifiedProperties();
    }
}
