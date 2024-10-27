using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentValue<T>
{
    private readonly string key;
    private readonly PersistenceMode mode;

    private SaveState saveState;

    public string Key => key;

    public PersistentValue(string key, PersistenceMode mode)
    {
        this.key = key;
        this.mode = mode;
    }

    private void FindSaveState()
    {
        if (!SaveState.TryGetOrFindInstance(mode, out saveState))
        {
            throw new System.Exception($"SaveState with mode {mode} not found");
        }
    }

    public void AddListener(ListenerLifetime lifetime, System.Action<bool, T> action, bool doInitialCall = true)
    {
        if (saveState == null) FindSaveState();
        saveState.AddListener(lifetime, key, action, doInitialCall);
    }

    public void AddSceneListener(System.Action<bool, T> action, bool doInitialCall = true)
    {
        AddListener(ListenerLifetime.Scene, action, doInitialCall);
    }

    public void AddGlobalListener(System.Action<bool, T> action, bool doInitialCall = true)
    {
        AddListener(ListenerLifetime.Global, action, doInitialCall);
    }

    public void RemoveListener(ListenerLifetime lifetime, System.Action<bool, T> action)
    {
        if (saveState == null) FindSaveState();
        saveState.RemoveListener(lifetime, key, action);
    }

    public void RemoveSceneListener(System.Action<bool, T> action)
    {
        RemoveListener(ListenerLifetime.Scene, action);
    }

    public void RemoveGlobalListener(System.Action<bool, T> action)
    {
        RemoveListener(ListenerLifetime.Global, action);
    }

    public bool TryGet(out T value)
    {
        if (saveState == null) FindSaveState();
        return saveState.TryGet(key, out value);
    }

    public T Get()
    {
        if (saveState == null) FindSaveState();
        if (!saveState.TryGet(key, out T value))
        {
            throw new System.Exception($"PersistentValue.Get: {key} not found");
        }
        return value;
    }

    public T GetOrDefault(T defaultValue)
    {
        if (saveState == null) FindSaveState();
        if (!saveState.TryGet(key, out T value))
        {
            return defaultValue;
        }
        return value;
    }

    public T GetOrSetDefault(T defaultValue)
    {
        if (saveState == null) FindSaveState();
        if (!saveState.TryGet(key, out T value))
        {
            saveState.Set(key, defaultValue);
            saveState.TryGet(key, out value);
        }
        return value;
    }

    public ref T GetRef()
    {
        if (saveState == null) FindSaveState();
        if (!saveState.TryGetBox(key, out SaveState.Box<T> box))
        {
            throw new System.Exception($"PersistentValue.Get: {key} not found");
        }
        return ref box.value;
    }

    public ref T GetOrCreateRef(T defaultValue)
    {
        if (saveState == null) FindSaveState();
        if (!saveState.TryGetBox(key, out SaveState.Box<T> box))
        {
            saveState.Set(key, defaultValue);
            saveState.TryGetBox(key, out box);
        }
        return ref box.value;
    }

    public void Set(T value)
    {
        if (saveState == null) FindSaveState();
        saveState.Set(key, value);
    }

    public void Remove()
    {
        if (saveState == null) FindSaveState();
        saveState.Remove(key);
    }

    public void MarkDirty()
    {
        if (saveState == null) FindSaveState();
        saveState.OnUpdate(key);
    }
}
