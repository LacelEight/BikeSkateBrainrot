using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// Complete loading screen UI component with progress bar and fade effects.
/// Setup: Create a Canvas with this script, add an Image for progress bar and Text for percentage.
/// </summary>
public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private Text percentageText;
    [SerializeField] private CanvasGroup fadePanel;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private SceneLoadHandler _sceneLoadHandler;

    /// <summary>
    /// Display loading screen and load the specified scene.
    /// </summary>
    public void LoadScene(string sceneName, float minLoadDuration = 1.5f)
    {
        if (fadePanel == null)
        {
            Debug.LogError("[LoadingScreenUI] CanvasGroup (fadePanel) not assigned!");
            return;
        }

        fadePanel.alpha = 1f;
        gameObject.SetActive(true);

        _sceneLoadHandler = new SceneLoadHandler(Services.SceneManager);

        // Register progress callback
        _sceneLoadHandler.OnProgress(OnLoadProgress);

        // Register completion callback
        _sceneLoadHandler.OnLoadComplete(OnLoadComplete);

        // Load the scene
        _sceneLoadHandler.LoadScene(sceneName, minLoadDuration);
    }

    /// <summary>
    /// Show the loading screen without loading (for manual control).
    /// </summary>
    public void Show()
    {
        fadePanel.alpha = 1f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide the loading screen with fade effect.
    /// </summary>
    public async void Hide()
    {
        await FadeOutAndHideAsync();
    }

    private void OnLoadProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = progress;
        }

        if (percentageText != null)
        {
            percentageText.text = $"{Mathf.Round(progress * 100f)}%";
        }

        Debug.Log($"[LoadingScreenUI] Progress: {progress:P0}");
    }

    private void OnLoadComplete()
    {
        Debug.Log("[LoadingScreenUI] Scene loaded, fading out...");
        FadeOutAndHideAsync().Forget();
    }

    private async UniTask FadeOutAndHideAsync()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / fadeDuration;
            fadePanel.alpha = fadeCurve.Evaluate(normalizedTime);
            await UniTask.Yield();
        }

        fadePanel.alpha = 0f;
        gameObject.SetActive(false);

        _sceneLoadHandler?.Cleanup();
    }

    /// <summary>
    /// Get the current load progress (0-1).
    /// </summary>
    public float GetProgress()
    {
        return _sceneLoadHandler?.GetProgress() ?? 0f;
    }

    /// <summary>
    /// Check if scene is currently loading.
    /// </summary>
    public bool IsLoading()
    {
        return _sceneLoadHandler?.IsLoading() ?? false;
    }
}
