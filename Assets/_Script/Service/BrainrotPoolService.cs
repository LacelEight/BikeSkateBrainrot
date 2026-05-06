using UnityEngine;
using Cysharp.Threading.Tasks;
using LacelSDK;

[CreateAssetMenu(fileName = "BrainrotPoolService", menuName = "Services/BrainrotPoolService")]
public class BrainrotPoolService : LacelService, IService
{
    public Transform PoolTransform;
    public async UniTaskVoid InitAsync()
    {

    }

    public void InjectPoolTransform(Transform poolTransform)
    {
        PoolTransform = poolTransform;
    }

    public void ReturnToPool(BrainrotManager brainrot)
    {
        brainrot.transform.SetParent(PoolTransform);
        brainrot.transform.position = Vector3.zero;
        brainrot.gameObject.SetActive(false);
    }
}
