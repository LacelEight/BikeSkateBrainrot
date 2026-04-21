using System;
using UnityEngine;

namespace LacelSDK
{
    public class LacelSystem : MonoBehaviour
    {
        public static LacelSystem instance { get; private set; }
#if use_service
        [SerializeField] private ServiceInstaller installer;
#endif
        public void Initialize(Action OnInitComplete)
        {
#if use_service
            installer.Install();
#endif
            OnInitComplete?.Invoke();
        }
    }

}
