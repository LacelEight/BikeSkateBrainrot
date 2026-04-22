using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : EditorWindow
{
    private bool autoSave = true;
    private Vector2 scroll;

    [MenuItem("Tools/Lacel Scene Manager")]
    public static void ShowWindow()
    {
        GetWindow<SceneSwitcher>("Scene Manager");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Scenes In Build", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        var scenes = EditorBuildSettings.scenes;

        for (int i = 0; i < scenes.Length; i++)
        {
            var scene = scenes[i];

            EditorGUILayout.BeginHorizontal("box");

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);

            // Load Scene button
            if (GUILayout.Button(sceneName, GUILayout.ExpandWidth(true), GUILayout.MinWidth(100)))
            {
                if (autoSave)
                {
                    SaveCurrentScene();
                }

                EditorSceneManager.OpenScene(scene.path);
            }

            // Focus button
            if (GUILayout.Button("Focus", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(80)))
            {
                var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);
                EditorGUIUtility.PingObject(asset);
                Selection.activeObject = asset;
            }

            // Delete button
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(80)))
            {
                RemoveSceneAtIndex(i);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        // Auto Save Toggle
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        autoSave = EditorGUILayout.Toggle("Auto Save", autoSave);
        EditorGUILayout.EndHorizontal();
    }

    private void SaveCurrentScene()
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveOpenScenes();
        }
    }

    private void RemoveSceneAtIndex(int index)
    {
        var scenes = EditorBuildSettings.scenes;
        var newScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
        newScenes.RemoveAt(index);
        EditorBuildSettings.scenes = newScenes.ToArray();
    }
}