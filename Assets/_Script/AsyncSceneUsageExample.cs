using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// Example usage of AsyncSceneManager system.
/// Shows different patterns for loading scenes asynchronously.
/// </summary>
public class AsyncSceneUsageExample : MonoBehaviour
{
    // Pattern 1: Simple async load with callback
    public void LoadSceneWithCallback()
    {
        var handler = new SceneLoadHandler(Services.SceneManager);
        
        handler.OnProgress(progress => 
        {
            Debug.Log($"Loading: {progress:P0}");
        });
        
        handler.OnLoadComplete(() => 
        {
            Debug.Log("Scene loaded!");
        });
        
        handler.LoadScene("MainMenuScene", minLoadTime: 2f);
    }

    // Pattern 2: Direct AsyncSceneManager usage (manual activation)
    public void LoadSceneManual()
    {
        var sceneManager = Services.SceneManager;
        
        sceneManager.OnLoadProgressChanged += progress => 
        {
            Debug.Log($"Progress: {progress:P0}");
        };
        
        sceneManager.OnLoadCompleted += () => 
        {
            Debug.Log("Loading complete - activating scene");
        };
        
        // Load scene but don't activate yet
        sceneManager.LoadSceneAsync("GameScene");
    }

    // Pattern 3: Delayed scene activation with UniTask
    public async void ActivateSceneAfterDelay()
    {
        var sceneManager = Services.SceneManager;
        
        sceneManager.LoadSceneAsync("GameScene");
        
        // Wait 3 seconds then activate
        await UniTask.Delay(3000);
        sceneManager.AllowSceneActivation();
    }

    // Pattern 4: Load with minimum duration (splash screen)
    public void LoadWithMinimumDuration()
    {
        var sceneManager = Services.SceneManager;
        
        // Scene will load in background, but won't activate for at least 2 seconds
        sceneManager.LoadSceneAsyncWithDelay("LoadingScene", minLoadTime: 2f);
    }

    // Pattern 5: Monitoring load state
    public void MonitorLoadState()
    {
        if (Services.SceneManager.IsLoading())
        {
            float progress = Services.SceneManager.GetLoadProgress();
            Debug.Log($"Currently loading... {progress:P0}");
        }
    }

    // Pattern 6: Advanced async pattern with UniTask (Recommended)
    public async void LoadSceneAsyncAdvanced()
    {
        var handler = new SceneLoadHandler(Services.SceneManager);
        
        // Setup progress tracking
        handler.OnProgress(progress => 
        {
            Debug.Log($"Loading: {progress:P0}");
        });

        handler.LoadScene("GameScene", minLoadTime: 1.5f);

        // Wait for completion asynchronously (no blocking)
        while (handler.IsLoading())
        {
            await UniTask.Delay(100); // Check every 100ms
        }

        Debug.Log("Scene fully loaded and activated!");
        handler.Cleanup();
    }
}
