using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSystem : MonoBehaviour
{
    private readonly List<Action> globalOnBeforeSceneUnload = new();
    private readonly List<Action> sceneOnBeforeSceneUnload = new();

    public static void AddOnBeforeSceneUnload(Action action, bool isGlobal)
    {
        if (isGlobal)
        {
            Globals<SceneSystem>.Instance.globalOnBeforeSceneUnload.Add(action);
        }
        else
        {
            Globals<SceneSystem>.Instance.sceneOnBeforeSceneUnload.Add(action);
        }
    }

    public static void RemoveOnBeforeSceneUnload(Action action, bool isGlobal)
    {
        if (isGlobal)
        {
            Globals<SceneSystem>.Instance.globalOnBeforeSceneUnload.Remove(action);
        }
        else
        {
            Globals<SceneSystem>.Instance.sceneOnBeforeSceneUnload.Remove(action);
        }
    }

    private void Awake()
    {
        Globals<SceneSystem>.RegisterOrDestroy(this);
    }

    private void OnSwitchScene()
    {
        var globalCount = globalOnBeforeSceneUnload.Count;
        var sceneCount = sceneOnBeforeSceneUnload.Count;

        foreach (var action in globalOnBeforeSceneUnload)
        {
            action?.Invoke();
        }
        foreach (var action in sceneOnBeforeSceneUnload)
        {
            action?.Invoke();
        }
        Debug.Log($"Invoked {globalCount} + {sceneCount} actions.");
        sceneOnBeforeSceneUnload.Clear();
        Debug.Log($"Now present: {globalOnBeforeSceneUnload.Count} + {sceneOnBeforeSceneUnload.Count} actions.");
    }

    public static void LoadScene(SceneObject scene)
    {
        Globals<SceneSystem>.Instance.OnSwitchScene();
        SceneManager.LoadScene(scene.sceneName);
    }
}
