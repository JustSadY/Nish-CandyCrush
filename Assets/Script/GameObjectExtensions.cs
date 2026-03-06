using UnityEngine;
using System;

public static class GameObjectExtensions
{
    public static T AddComponent<T>(this GameObject gameObject, Action<T> initAction) where T : Component
    {
        T component = gameObject.AddComponent<T>();
        initAction?.Invoke(component);
        return component;
    }
}