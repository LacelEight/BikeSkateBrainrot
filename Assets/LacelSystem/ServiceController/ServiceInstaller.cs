using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LacelSDK
{
    [CreateAssetMenu(fileName = "ServiceInstaller", menuName = "LacelSDK/ServiceInstaller")]
    public class ServiceInstaller : ScriptableObject
    {
        [Searchable]
        [SerializeField] private List<LacelService> services;
        public List<LacelService> ServiceList => services;

        public void Install()
        {
            foreach (var service in services)
            {
                if (service is IService s)
                {
                    s.InitAsync();
                    Services.Register(s);
                }
            }
        }
    }
}
