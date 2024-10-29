using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Persistence;
using UnityEngine;
using Yeast;

public class SaveState : MonoBehaviour
{
    private readonly static Dictionary<PersistenceMode, SaveState> instances = new();

    public PersistenceMode mode;

    private SaveData saveData = SaveData.New();

    private readonly Dictionary<string, IListener> listeners = new();
    private readonly Dictionary<string, IBox> data = new();
    private bool isLoaded = false;
    private string path;

    private void Awake()
    {
        if (!AddInstanceOrDestroy()) return;

        if (mode == PersistenceMode.GlobalRuntime || mode == PersistenceMode.GlobalPersistence)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (mode == PersistenceMode.ScenePersistence)
        {
            SceneSystem.AddOnBeforeSceneUnload(() => Save(), false);
        }

        SceneSystem.AddOnBeforeSceneUnload(() => ResetSceneListeners(), mode != PersistenceMode.ScenePersistence);
    }

    private bool AddInstanceOrDestroy()
    {
        if (TryGetInstance(mode, out var instance))
        {
            if (instance == this) return true;
            Destroy(this);
            return false;
        }

        print($"Adding instance for mode: {mode}");
        instances[mode] = this;
        path = mode == PersistenceMode.GlobalRuntime ? "" : GetPath();
        Load();
        return true;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    public string GetPath()
    {
        return mode switch
        {
            PersistenceMode.GlobalPersistence => "SaveState_Global.json",
            PersistenceMode.ScenePersistence => $"SaveState_{gameObject.scene.buildIndex}.json",
            _ => throw new Exception("Invalid mode")
        };
    }

    private static bool TryGetInstance(PersistenceMode mode, out SaveState instance)
    {
        return instances.TryGetValue(mode, out instance) && instance != null;
    }

    public static bool TryGetOrFindInstance(PersistenceMode mode, out SaveState instance)
    {
        if (TryGetInstance(mode, out instance)) return true;

        var allSaveStates = FindObjectsOfType<SaveState>();
        foreach (var saveState in allSaveStates)
        {
            saveState.AddInstanceOrDestroy();
        }

        return TryGetInstance(mode, out instance);
    }

    private void ResetSceneListeners()
    {
        foreach (var (key, listener) in listeners)
        {
            print($"Resetting listeners for key: '{key}'");
            listener.ResetSceneListeners();
        }
    }

    private void Save()
    {
        if (mode == PersistenceMode.GlobalRuntime) return;
        if (!isLoaded) throw new Exception($"SaveState '{path}' has not loaded yet.");

        foreach (var (key, box) in data)
        {
            saveData.data[key] = box.GetValue().ToJson();
        }
        Debug.Log($"Saving SaveState '{path}' " + mode);
        if (saveData.data.Count == 0) return;

        JsonPersistence.Save(path, saveData);
    }

    private void Load()
    {
        if (mode == PersistenceMode.GlobalRuntime) return;
        if (isLoaded) return;
        Debug.Log($"Loading SaveState '{path}' " + mode);

        saveData = JsonPersistence.LoadDefault(path, SaveData.New());

        isLoaded = true;
    }

    public void Delete()
    {
        if (mode == PersistenceMode.GlobalRuntime) return;

        JsonPersistence.Delete(path);

        saveData.data.Clear();
        foreach (var key in data.Keys.ToList())
        {
            data.Remove(key);
            OnUpdate(key);
        }
    }

    public bool TryGetBox<T>(string key, out Box<T> box)
    {
        if (!TryLoadKey<T>(key))
        {
            box = null;
            return false;
        }

        if (data.TryGetValue(key, out var anyBox) && anyBox is Box<T> typedBox)
        {
            box = typedBox;
            return true;
        }
        box = null;
        return false;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (TryGetBox<T>(key, out var box))
        {
            value = box.value;
            return true;
        }
        value = default;
        return false;
    }

    public void Set<T>(string key, T value)
    {
        if (data.TryGetValue(key, out var box))
        {
            if (box is Box<T> typedBox)
            {
                typedBox.value = value;
            }
            else
            {
                data[key] = new Box<T>(value);
            }
        }
        else
        {
            data.Add(key, new Box<T>(value));
        }
        OnUpdate(key);
    }

    public void Remove(string key)
    {
        saveData.data.Remove(key);
        if (data.Remove(key))
        {
            OnUpdate(key);
        }
    }

    public void OnUpdate(string key)
    {
        if (listeners.TryGetValue(key, out var listener))
        {
            if (data.TryGetValue(key, out var box))
            {
                listener.OnValueChanged(true, box.GetValue());
            }
            else
            {
                listener.OnValueChanged(false, null);
            }
        }
    }

    private bool TryLoadKey<T>(string key)
    {
        if (data.TryGetValue(key, out var anyBox))
        {
            return anyBox is Box<T>;
        }
        else if (saveData.data.ContainsKey(key))
        {
            Debug.Log("loading: " + key + " = " + saveData.data[key]);
            if (saveData.data[key].TryFromJson(out T value))
            {
                data[key] = new Box<T>(value);
                return true;
            }
            else
            {
                Debug.LogError($"Failed to load key '{key}' from save data.");
                saveData.data[key].FromJson<T>();
            }
        }
        return false;
    }

    public void AddListener<T>(ListenerLifetime lifetime, string key, Action<bool, T> action, bool doInitialCall)
    {
        TryLoadKey<T>(key);
        if (!listeners.TryGetValue(key, out var listener))
        {
            listener = new Listener<T>();
            listeners.Add(key, listener);
        }
        (listener as Listener<T>).AddListener(lifetime, action);
        if (doInitialCall)
        {
            if (TryGet(key, out T value)) action(true, value);
            else action(false, default);
        }
    }

    public void RemoveListener<T>(ListenerLifetime lifetime, string key, Action<bool, T> action)
    {
        TryLoadKey<T>(key);
        if (listeners.TryGetValue(key, out var listener))
        {
            (listener as Listener<T>).RemoveListener(lifetime, action);
        }
    }

    private interface IBox
    {
        public object GetValue();
    }

    public class Box<T> : IBox
    {
        public T value;

        public Box(T value)
        {
            this.value = value;
        }

        public object GetValue()
        {
            return value;
        }
    }

    private interface IListener
    {
        public void OnValueChanged(bool isPresent, object val);
        public void ResetSceneListeners();
    }

    public class Listener<T> : IListener
    {
        public Action<bool, T> sceneOnValueChanged;
        public Action<bool, T> globalOnValueChanged;

        public void OnValueChanged(bool isPresent, object val)
        {
            sceneOnValueChanged?.Invoke(isPresent, isPresent ? (T)val : default);
            globalOnValueChanged?.Invoke(isPresent, isPresent ? (T)val : default);
        }

        public void AddListener(ListenerLifetime lifetime, Action<bool, T> action)
        {
            if (lifetime == ListenerLifetime.Global) globalOnValueChanged += action;
            else sceneOnValueChanged += action;
        }

        public void RemoveListener(ListenerLifetime lifetime, Action<bool, T> action)
        {
            if (lifetime == ListenerLifetime.Global) globalOnValueChanged -= action;
            else sceneOnValueChanged -= action;
        }

        public void ResetSceneListeners()
        {
            sceneOnValueChanged = null;
        }
    }

    public struct SaveData
    {
        public Dictionary<string, string> data;

        public static SaveData New()
        {
            return new SaveData() { data = new() };
        }
    }
}

public enum PersistenceMode { GlobalPersistence, ScenePersistence, GlobalRuntime }

public enum ListenerLifetime { Global, Scene }
