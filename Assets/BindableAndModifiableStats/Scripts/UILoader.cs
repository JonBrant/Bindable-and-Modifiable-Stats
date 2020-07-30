using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

public class UILoader : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField] List<SceneAsset> UIScenes = new List<SceneAsset>();
    private List<string> scenesInBuild = new List<string>();

    private void Awake() {
        SetEditorBuildSettingsScenes();
    }
#endif

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            if (SceneManager.GetSceneByName("UI One").isLoaded == false) {
                SceneManager.LoadSceneAsync("UI One", LoadSceneMode.Additive);
                if (SceneManager.GetSceneByName("UI Two").isLoaded) {
                    SceneManager.UnloadSceneAsync("UI Two");
                }
            }
            else
                SceneManager.UnloadSceneAsync("UI One");
        }

        if (Input.GetKeyDown(KeyCode.F2)) {
            if (SceneManager.GetSceneByName("UI Two").isLoaded == false) {
                SceneManager.LoadSceneAsync("UI Two", LoadSceneMode.Additive);
                if (SceneManager.GetSceneByName("UI One").isLoaded) {
                    SceneManager.UnloadSceneAsync("UI One");
                }
            }
            else
                SceneManager.UnloadSceneAsync("UI Two");
        }
    }
#if UNITY_EDITOR
    public void SetEditorBuildSettingsScenes() {
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (!string.IsNullOrEmpty(scenePath)) {
                int lastSlash = scenePath.LastIndexOf("/");
                string sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);
                scenesInBuild.Add(sceneName);
            }
        }

        // Find valid Scene paths and make a list of EditorBuildSettingsScene
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();


        foreach (var sceneAsset in UIScenes) {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);

            int lastSlash = scenePath.LastIndexOf("/");
            string sceneNameToAdd = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);
            if (!string.IsNullOrEmpty(scenePath)) {
                if (!scenesInBuild.Contains(sceneNameToAdd)) {
                    editorBuildSettingsScenes.Add(sceneToAdd);
                    scenesInBuild.Add(sceneNameToAdd);
                }
            }
        }
        
        
        //Make sure to add all scenes already in list to new list, or they'll disappear.
        foreach (var scene in EditorBuildSettings.scenes) {
            if (!string.IsNullOrEmpty(scene.path)) {
                var sceneToAdd = new EditorBuildSettingsScene(scene.path, true);
                editorBuildSettingsScenes.Add(sceneToAdd);
            }
        }

        // Set the Build Settings window Scene list
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
    }
    #endif
}