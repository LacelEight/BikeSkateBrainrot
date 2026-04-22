using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LacelSDK
{
    public class Services
    {
        private static Dictionary<Type, object> services = new();
        public static Dictionary<Type, object>  RegisteredServices => services;

        public static void Register<T>(T service)
        {
            services[service.GetType()] = service;
        }

        public static T Get<T>() where T : class
        {
            if (services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
            return null;
        }
    }
}
