# Async Scene Manager System - Documentation

## Overview
This system provides asynchronous scene loading to prevent lag and frame rate spikes during scene transitions. Instead of blocking the frame when loading heavy scenes, the system loads in the background and notifies your code via events and callbacks.

## Components

### 1. AsyncSceneManager
Core component that handles async scene operations. It's a MonoBehaviour that:
- Loads scenes asynchronously in the background
- Provides real-time progress tracking
- Offers event callbacks for load state changes
- Prevents automatic scene activation for fine-grained control

**Key Methods:**
```csharp
LoadSceneAsync(string sceneName)              // Load without auto-activation
LoadSceneAsyncWithDelay(sceneName, minTime)   // Load with minimum display duration
AllowSceneActivation()                        // Manually activate loaded scene
GetLoadProgress()                             // Get current progress (0-1)
IsLoading()                                   // Check if loading
CancelLoad()                                  // Cancel current load
```

**Events:**
```csharp
OnLoadStarted           // Scene load begins
OnLoadProgressChanged   // Progress updated (float: 0-1)
OnLoadCompleted         // Scene is fully loaded
OnLoadFailed            // Load failed (string: error message)
```

### 2. SceneLoadHandler
Wrapper around AsyncSceneManager that simplifies common usage patterns:
- Manages callback queues
- Provides cleaner API
- Handles event subscription/cleanup

**Key Methods:**
```csharp
LoadScene(sceneName, minLoadTime = 0f)  // Load scene
OnLoadComplete(Action callback)           // Register completion callback
OnProgress(Action<float> callback)        // Register progress callback
GetProgress()                             // Get current progress
IsLoading()                               // Check if loading
ActivateScene()                           // Manually activate
```

### 3. Services
Updated to automatically initialize AsyncSceneManager:
```csharp
Services.SceneManager  // Access the scene manager globally
```

## Usage Patterns

### Pattern 1: Simple Scene Load with Progress
```csharp
var handler = new SceneLoadHandler(Services.SceneManager);

handler.OnProgress(progress => 
{
    progressBar.value = progress;
});

handler.OnLoadComplete(() => 
{
    Debug.Log("Scene ready!");
});

handler.LoadScene("GameScene", minLoadTime: 1.5f);
```

### Pattern 2: Direct Control with Events
```csharp
Services.SceneManager.OnLoadProgressChanged += (progress) =>
{
    loadingText.text = $"Loading: {progress:P0}";
};

Services.SceneManager.OnLoadCompleted += () =>
{
    loadingScreen.FadeOut();
};

Services.SceneManager.LoadSceneAsync("GameScene");
```

### Pattern 3: Deferred Activation
```csharp
// Load scene but don't activate yet
Services.SceneManager.LoadSceneAsync("MenuScene");

// Do something while loading...
yield return new WaitForSeconds(2f);

// Now activate the scene
Services.SceneManager.AllowSceneActivation();
```

### Pattern 4: Load with Minimum Duration
Ensures loading screen displays for at least X seconds (prevents quick flashes):
```csharp
Services.SceneManager.LoadSceneAsyncWithDelay("MainMenu", minLoadTime: 2f);
```

## LoadingManager Integration

The updated `LoadingManager` automatically:
1. Initializes the scene loading system
2. Tracks progress
3. Handles scene transitions after LacelSystem initialization

**To use:**
1. Edit `LoadingManager.cs` and change `"GameScene"` to your actual first game scene name
2. Create a loading UI with a progress bar (optional)
3. Connect the progress callback to your UI

Example UI integration:
```csharp
handler.OnProgress(progress => 
{
    loadingBar.fillAmount = progress;  // For Image with Fill
    loadingText.text = $"{progress:P0}";
});
```

## Best Practices

### 1. Prevent Frame Rate Spikes
- Use async loading for large scenes
- Never load synchronously in Update/LateUpdate
- Always use LoadSceneAsync or LoadSceneAsyncWithDelay

### 2. Splash Screen Duration
```csharp
// Keep loading screen visible for minimum 1.5 seconds
handler.LoadScene("GameScene", minLoadTime: 1.5f);
```

### 3. Progress Tracking
- Progress is capped at 90% until scene activation (Unity behavior)
- After activation, progress reaches 100%
- Use real-time progress for accurate UI feedback

### 4. Error Handling
```csharp
Services.SceneManager.OnLoadFailed += (error) =>
{
    Debug.LogError($"Failed to load scene: {error}");
    // Show error UI to player
};
```

### 5. Memory Management
- Cleanup handler after use
- Unsubscribe from events
- Use `DontDestroyOnLoad` for persistent managers

## Build Settings
Ensure all scenes you want to load asynchronously are added to:
**File > Build Settings > Scenes In Build**

## Performance Impact

### Without Async (Synchronous)
- Scene loads: **0-500ms** (depending on size)
- Frame rate: **Drops significantly** during load
- User experience: **Stutters/lag visible**

### With Async (This System)
- Scene loads: **Spread over multiple frames** (typically 2-10 frames)
- Frame rate: **Stable** (small frame time variance)
- User experience: **Smooth with animated loading screen**

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Scene doesn't load | Ensure scene is in Build Settings |
| Progress shows 0.9 and stays there | Normal behavior - waiting for activation |
| Script errors about null SceneManager | Ensure Services initialization ran first |
| Loading takes too long | Check scene size and optimize assets |

## Example: Complete Loading Screen

```csharp
public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Text percentText;
    [SerializeField] private CanvasGroup canvasGroup;

    public void ShowLoadingScreen(string sceneName)
    {
        canvasGroup.alpha = 1f;
        var handler = new SceneLoadHandler(Services.SceneManager);
        
        handler.OnProgress(progress =>
        {
            progressBar.fillAmount = progress;
            percentText.text = $"{progress:P0}";
        });
        
        handler.OnLoadComplete(() =>
        {
            StartCoroutine(FadeOut());
        });
        
        handler.LoadScene(sceneName, minLoadTime: 1.5f);
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
```
