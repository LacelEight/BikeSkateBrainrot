using System;
using UnityEngine;

public class BrainrotManager : MonoBehaviour
{
    public BrainrotCanvas BrainrotCanvas;
    public BrainrotConfig BrainrotConfig;
    public static Action<BrainrotManager> OnPlayerEnterBrainrotField;
    public static Action<BrainrotManager> OnPlayerExitBrainrotField;
    public static Action<BrainrotManager> OnTempBrainrotCollected;

    private bool IsCollected = false;

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

    public void CollectBrainrot()
    {
        IsCollected = true;
        BrainrotCanvas.DisableCanvas();
        OnTempBrainrotCollected?.Invoke(this);
        OnPlayerExitBrainrotField?.Invoke(this);
    }
}
