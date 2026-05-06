using UnityEngine;
using Cysharp.Threading.Tasks;
using LacelSDK;

[CreateAssetMenu(fileName = "BrainrotPoolService", menuName = "Services/BrainrotPoolService")]
public class BrainrotPoolService : LacelService, IService
{
    public async UniTaskVoid InitAsync()
    {
        throw new System.NotImplementedException();
    }
}
