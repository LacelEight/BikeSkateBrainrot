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

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }

            if (!instance)
            {
                instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        public static void Initialize(Action OnInitComplete)
        {
#if use_service
            instance.installer.Install();
#endif

            OnInitComplete?.Invoke();
        }

        public static T GetService<T>() where T : class
        {
            return instance?.Get<T>();
        }

        public T Get<T>() where T : class
        {
            return Services.Get<T>();
        }
    }

}
