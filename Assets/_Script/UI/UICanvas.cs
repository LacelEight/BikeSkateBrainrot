using System;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public GameObject Joystick;
    public Button JumpButton;
    public Button BrainrotButton;
    public TMP_Text fpsTmp;

    private float smoothedDeltaTime;

    private void Awake()
    {
        BrainrotManager.OnPlayerEnterBrainrotField += OnPlayerEnterBrainrotField;
        BrainrotManager.OnPlayerExitBrainrotField += OnPlayerExitBrainrotField;
        BrainrotButton.onClick.AddListener(OnBrainrotButtonClick);
    }

    private void OnDestroy()
    {
        BrainrotManager.OnPlayerEnterBrainrotField -= OnPlayerEnterBrainrotField;
        BrainrotManager.OnPlayerExitBrainrotField -= OnPlayerExitBrainrotField;
        BrainrotButton.onClick.RemoveListener(OnBrainrotButtonClick);
    }

    private void OnBrainrotButtonClick()
    {
        Services.InventoryService.CollectTempBrainrot();
    }

    private void FixedUpdate()
    {
        fpsTmp.text = $"FPS: {GetFPS()}";
    }

    public float GetFPS()
    {
        return 1f / Time.deltaTime;
    }

    private void OnPlayerExitBrainrotField(BrainrotManager manager)
    {
        UniTask.Create(async () =>
        {
            await UniTask.DelayFrame(1);
            if (Services.InventoryService.BrainrotInTouch.Count == 0)
                BrainrotButton.gameObject.SetActive(false);
        });
    }

    private void OnPlayerEnterBrainrotField(BrainrotManager manager)
    {
        UniTask.Create(async () =>
        {
            await UniTask.DelayFrame(1);
            if (Services.InventoryService.BrainrotInTouch.Count > 0)
                BrainrotButton.gameObject.SetActive(true);
        });
    }
}
