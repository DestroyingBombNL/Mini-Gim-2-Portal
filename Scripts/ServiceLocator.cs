using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (!services.ContainsKey(type))
        {
            services.Add(type, service);
        }
        else
        {
            Debug.LogWarning($"Service of type {type} is already registered.");
        }
    }

    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
        {
            return service as T;
        }

        Debug.LogError($"Service of type {type} not found.");
        return null;
    }

    public static void Clear()
    {
        services.Clear();
    }
}
