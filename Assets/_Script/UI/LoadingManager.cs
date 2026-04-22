using System;
using LacelSDK;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private float _minLoadDuration = 1f; // Minimum splash screen display time
    private SceneLoadHandler _sceneLoadHandler;

    private void Start()
    {
        LacelSystem.Initialize(OnSystemInitialized);
    }

    private void OnSystemInitialized()
    {
        _sceneLoadHandler = new SceneLoadHandler(Services.SceneManager);
        _sceneLoadHandler.OnProgress(OnLoadProgress);
        _sceneLoadHandler.OnLoadComplete(OnSceneLoadComplete);

        LoadGameScene();
    }

    private void LoadGameScene()
    {
        _sceneLoadHandler.LoadScene("GameScene", _minLoadDuration);
    }

    private void OnLoadProgress(float progress)
    {
        // Update UI progress bar here
        // Example: Update loading bar
        // LoadingUI.SetProgress(progress);
    }

    private void OnSceneLoadComplete()
    {
        _sceneLoadHandler?.Cleanup();
    }
}
