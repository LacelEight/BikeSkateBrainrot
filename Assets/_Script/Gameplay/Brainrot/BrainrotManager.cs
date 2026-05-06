using System;
using System.ComponentModel;
using LacelSDK;
using Sirenix.OdinInspector;
using UnityEngine;

public class BrainrotManager : MonoBehaviour
{
    public BrainrotCanvas BrainrotCanvas;
    [Sirenix.OdinInspector.ReadOnly]
    public MeshRenderer MeshRenderer;
    public MeshFilter MeshFilter;
    public BrainrotData BrainrotData;
    public static Action<BrainrotManager> OnPlayerEnterBrainrotField;
    public static Action<BrainrotManager> OnPlayerExitBrainrotField;
    public static Action<BrainrotManager> OnBrainrotCollected;
    public static Action<BrainrotManager> OnBrainrotOutOfTime;

    private bool IsCollected = false;

    private int currentTime = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsCollected)
        {
            OnPlayerEnterBrainrotField?.Invoke(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !IsCollected)
        {
            OnPlayerExitBrainrotField?.Invoke(this);
        }
    }

    private void Awake()
    {
        TickService.OnTick += OnTick;
    }

    private void OnDestroy()
    {
        TickService.OnTick -= OnTick;
    }

    private void OnTick()
    {
        CountdownTime();
    }

    private void CountdownTime()
    {
        if (IsCollected) return;
        currentTime--;
        if (currentTime < 0) currentTime = 0;
        BrainrotCanvas.TimeTMP.text = $"{currentTime}s";
        if (currentTime <= 0)
        {
            ReturnToPool();
        }
    }

    public void CollectBrainrot()
    {
        IsCollected = true;
        BrainrotCanvas.DisableCanvas();
        OnBrainrotCollected?.Invoke(this);
        OnPlayerExitBrainrotField?.Invoke(this);
    }


    private void ReturnToPool()
    {
        OnBrainrotOutOfTime?.Invoke(this);
    }

    public void InitializeFromSpawner(BrainrotConfig brainrotConfig, Modifier modifier)
    {
        BrainrotData.BrainrotConfig = brainrotConfig;
        MeshRenderer.material = brainrotConfig.GetMaterial(modifier);
        MeshFilter.mesh = brainrotConfig.Mesh;
        double min = BrainrotData.BrainrotConfig.BaseValueRange.Min;
        double max = BrainrotData.BrainrotConfig.BaseValueRange.Max;
        System.Random random = new System.Random();
        BrainrotData.Value = min + (max - min) * random.NextDouble();
        BindUIData();
    }

    private void BindUIData()
    {
        BrainrotCanvas.TimeTMP.text = $"{currentTime}s";
        BrainrotCanvas.ModifierTMP.text = BrainrotData.Modifier.ToString();
        BrainrotCanvas.NameTMP.text = BrainrotData.BrainrotConfig.Type.ToString();
        BrainrotCanvas.RarityTMP.text = BrainrotData.BrainrotConfig.Rarity.ToString();
        BrainrotCanvas.ValueTMP.text = BigNumberFormatter.Format(BrainrotData.Value).ToString();
    }
}
