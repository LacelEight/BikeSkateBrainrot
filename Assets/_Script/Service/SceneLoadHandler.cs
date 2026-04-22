using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadHandler
{
    private AsyncSceneManager _sceneManager;
    private Queue<Action> _onLoadCompletedCallbacks = new Queue<Action>();
    private Queue<Action<float>> _onProgressCallbacks = new Queue<Action<float>>();

    public SceneLoadHandler(AsyncSceneManager sceneManager)
    {
        _sceneManager = sceneManager;
        
        // Subscribe to scene manager events
        _sceneManager.OnLoadStarted += HandleLoadStarted;
        _sceneManager.OnLoadProgressChanged += HandleProgressChanged;
        _sceneManager.OnLoadCompleted += HandleLoadCompleted;
        _sceneManager.OnLoadFailed += HandleLoadFailed;
    }

    /// <summary>
    /// Register a callback to be invoked when scene loading completes.
    /// </summary>
    public void OnLoadComplete(Action callback)
    {
        _onLoadCompletedCallbacks.Enqueue(callback);
    }

    /// <summary>
    /// Register a callback to track load progress (0-1).
    /// </summary>
    public void OnProgress(Action<float> callback)
    {
        _onProgressCallbacks.Enqueue(callback);
    }

    /// <summary>
    /// Load scene asynchronously with a minimum display time (for splash screens).
    /// </summary>
    public void LoadScene(string sceneName, float minLoadTime = 0f)
    {
        if (minLoadTime > 0f)
        {
            _sceneManager.LoadSceneAsyncWithDelay(sceneName, minLoadTime);
        }
        else
        {
            _sceneManager.LoadSceneAsync(sceneName);
        }
    }

    /// <summary>
    /// Get the current load progress.
    /// </summary>
    public float GetProgress()
    {
        return _sceneManager.GetLoadProgress();
    }

    /// <summary>
    /// Check if currently loading.
    /// </summary>
    public bool IsLoading()
    {
        return _sceneManager.IsLoading();
    }

    /// <summary>
    /// Allow manual scene activation after LoadSceneAsync.
    /// </summary>
    public void ActivateScene()
    {
        _sceneManager.AllowSceneActivation();
    }

    private void HandleLoadStarted()
    {
        Debug.Log("[SceneLoadHandler] Scene load started");
    }

    private void HandleProgressChanged(float progress)
    {
        // Invoke all progress callbacks
        while (_onProgressCallbacks.Count > 0)
        {
            var callback = _onProgressCallbacks.Dequeue();
            callback?.Invoke(progress);
        }
    }

    private void HandleLoadCompleted()
    {
        Debug.Log("[SceneLoadHandler] Scene load completed");
        
        // Invoke all completion callbacks
        while (_onLoadCompletedCallbacks.Count > 0)
        {
            var callback = _onLoadCompletedCallbacks.Dequeue();
            callback?.Invoke();
        }
    }

    private void HandleLoadFailed(string error)
    {
        Debug.LogError($"[SceneLoadHandler] Load failed: {error}");
    }

    public void Cleanup()
    {
        if (_sceneManager != null)
        {
            _sceneManager.OnLoadStarted -= HandleLoadStarted;
            _sceneManager.OnLoadProgressChanged -= HandleProgressChanged;
            _sceneManager.OnLoadCompleted -= HandleLoadCompleted;
            _sceneManager.OnLoadFailed -= HandleLoadFailed;
        }

        _onLoadCompletedCallbacks.Clear();
        _onProgressCallbacks.Clear();
    }
}
