using System;
using Cysharp.Threading.Tasks;
using LacelSDK;
using UnityEngine;

[CreateAssetMenu(fileName = "TickService", menuName = "Services/TickService")]
public class TickService : LacelService, IService
{
    public static Action OnTick;
    public static Action OnCustomUpdate;
    public async UniTaskVoid InitAsync()
    {
        StartTickLoop().Forget();
        CustomLoop().Forget();
    }

    private async UniTask StartTickLoop()
    {
        while (true)
        {
            OnTick?.Invoke();
            await UniTask.Delay(1000);
        }
    }

    private async UniTask CustomLoop()
    {
        while (true)
        {
            OnCustomUpdate?.Invoke();
            await UniTask.Delay(40);
        }
    }
}
