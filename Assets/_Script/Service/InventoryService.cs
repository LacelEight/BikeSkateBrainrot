using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LacelSDK;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryService", menuName = "Services/InventoryService")]
public class InventoryService : LacelService, IService
{
    [HideInInspector]
    public List<BrainrotManager> BrainrotInTouch = new();

    [HideInInspector]
    public List<GameObject> InventoryList = new();

    [HideInInspector]
    public List<BrainrotManager> TempInventoryBrainrot = null;
    public async UniTaskVoid InitAsync()
    {
        BrainrotManager.OnPlayerEnterBrainrotField += OnPlayerEnterBrainrotField;
        BrainrotManager.OnPlayerExitBrainrotField += OnPlayerExitBrainrotField;
    }

    public void CollectTempBrainrot()
    {
        if (BrainrotInTouch.Count <= 0)
        {
            Debug.LogError("No brainrot in touch");
            return;
        }

        TempInventoryBrainrot.Add(BrainrotInTouch[0]);
        foreach (var brainrot in TempInventoryBrainrot)
        {
            brainrot.CollectBrainrot();
        }
    }

    private void OnPlayerEnterBrainrotField(BrainrotManager manager)
    {
        BrainrotInTouch.Add(manager);
    }

    private void OnPlayerExitBrainrotField(BrainrotManager manager)
    {
        BrainrotInTouch.Remove(manager);
    }

    public void AddTempToInventory()
    {
        if (TempInventoryBrainrot == null) return;
        foreach (var brainrot in TempInventoryBrainrot)
        {
            InventoryList.Add(brainrot.gameObject);
            InventoryList.Add(brainrot.gameObject);
            Services.BrainrotPoolService.ReturnToPool(brainrot);
            Debug.LogError($"Added Temp to Inventory: {brainrot.gameObject.name}");
        }
        TempInventoryBrainrot.Clear();
    }

    public void ClearTempInventory()
    {
        if (TempInventoryBrainrot == null) return;
        foreach (var brainrot in TempInventoryBrainrot)
        {
            Services.BrainrotPoolService.ReturnToPool(brainrot);
            Debug.LogError($"Added Temp to Inventory: {brainrot.gameObject.name}");
        }
        TempInventoryBrainrot.Clear();
    }
}
