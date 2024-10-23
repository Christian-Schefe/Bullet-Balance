using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals<T> where T : Object
{
    public static T Instance => GetInstance();
    private static T instance;

    private static T GetInstance()
    {
        if (instance == null)
        {
            instance = Object.FindObjectOfType<T>();
            if (instance == null)
            {
                Debug.LogError($"Globals<{typeof(T).Name}>: No instance found.");
            }
        }
        return instance;
    }

    public static bool RegisterOrDestroy(T obj)
    {
        if (instance == null)
        {
            instance = obj;
            return true;
        }
        else if (instance == obj)
        {
            return true;
        }
        else
        {
            Object.Destroy(obj);
            return false;
        }
    }
}
