using Cysharp.Threading.Tasks;
using UnityEngine;

namespace LacelSDK
{
    [CreateAssetMenu(fileName = "AudioService", menuName = "Services/AudioService")]
    public class AudioServices : LacelService, IService
    {
        public async UniTaskVoid InitAsync()
        {
            
        }
    }
}
