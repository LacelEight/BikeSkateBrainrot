using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncSceneManager : MonoBehaviour
{
    private AsyncOperation _currentAsyncLoad;
    private float _loadProgress;
    private bool _isLoading;

    public event Action<float> OnLoadProgressChanged;
    public event Action OnLoadStarted;
    public event Action OnLoadCompleted;
    public event Action<string> OnLoadFailed;

    /// <summary>
    /// Load a scene asynchronously without automatically activating it.
    /// Call AllowSceneActivation() to complete the load.
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        if (_isLoading)
        {
            Debug.LogWarning($"Already loading a scene. Cannot load '{sceneName}'");
            return;
        }

        _isLoading = true;
        OnLoadStarted?.Invoke();
        _currentAsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        
        if (_currentAsyncLoad == null)
        {
            OnLoadFailed?.Invoke($"Scene '{sceneName}' not found in build settings");
            _isLoading = false;
            return;
        }

        _currentAsyncLoad.allowSceneActivation = false;
    }

    /// <summary>
    /// Load a scene asynchronously and allow activation after optional delay.
    /// </summary>
    public async void LoadSceneAsyncWithDelay(string sceneName, float minLoadTime = 0f)
    {
        if (_isLoading)
        {
            Debug.LogWarning($"Already loading a scene. Cannot load '{sceneName}'");
            return;
        }

        await LoadWithDelayAsync(sceneName, minLoadTime);
    }

    /// <summary>
    /// Allow the loaded scene to activate (must call LoadSceneAsync first).
    /// </summary>
    public void AllowSceneActivation()
    {
        if (_currentAsyncLoad != null)
        {
            _currentAsyncLoad.allowSceneActivation = true;
        }
    }

    /// <summary>
    /// Get the current load progress (0-1).
    /// </summary>
    public float GetLoadProgress()
    {
        if (_currentAsyncLoad == null)
            return 0f;

        return _currentAsyncLoad.progress;
    }

    /// <summary>
    /// Check if scene is currently loading.
    /// </summary>
    public bool IsLoading()
    {
        return _isLoading;
    }

    /// <summary>
    /// Cancel the current loading operation and reload current scene.
    /// </summary>
    public void CancelLoad()
    {
        if (_currentAsyncLoad != null)
        {
            _currentAsyncLoad = null;
        }

        _isLoading = false;
    }

    private void Update()
    {
        if (!_isLoading || _currentAsyncLoad == null)
            return;

        // Progress is capped at 0.9 until scene activation
        _loadProgress = Mathf.Clamp01(_currentAsyncLoad.progress / 0.9f);
        OnLoadProgressChanged?.Invoke(_loadProgress);

        // Check if loading is complete (progress = 1 and not done yet)
        if (_currentAsyncLoad.isDone)
        {
            _isLoading = false;
            OnLoadCompleted?.Invoke();
        }
    }

    private async UniTask LoadWithDelayAsync(string sceneName, float minLoadTime)
    {
        float loadStartTime = Time.realtimeSinceStartup;
        _isLoading = true;
        OnLoadStarted?.Invoke();

        _currentAsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        if (_currentAsyncLoad == null)
        {
            OnLoadFailed?.Invoke($"Scene '{sceneName}' not found in build settings");
            _isLoading = false;
            return;
        }

        _currentAsyncLoad.allowSceneActivation = false;

        // Wait for loading to complete
        while (_currentAsyncLoad.progress < 0.9f)
        {
            _loadProgress = Mathf.Clamp01(_currentAsyncLoad.progress / 0.9f);
            OnLoadProgressChanged?.Invoke(_loadProgress);
            await UniTask.Yield();
        }

        // Wait for minimum load time (smooth loading screen display)
        float elapsedTime = Time.realtimeSinceStartup - loadStartTime;
        if (elapsedTime < minLoadTime)
        {
            await UniTask.Delay((int)((minLoadTime - elapsedTime) * 1000));
        }

        // Allow scene activation
        _currentAsyncLoad.allowSceneActivation = true;
        _isLoading = false;
        OnLoadCompleted?.Invoke();
    }
}
