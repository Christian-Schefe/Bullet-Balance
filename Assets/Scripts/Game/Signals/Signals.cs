using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISignal
{
    public void RemoveAllSceneListeners();
}

public static class Signals
{
    private static readonly SignalHub hub = new();

    public static T Get<T>() where T : ISignal, new() => hub.Get<T>();

    private class SignalHub
    {
        private readonly Dictionary<Type, ISignal> signals = new();

        public SignalHub()
        {
            SceneSystem.AddOnBeforeSceneUnload(RemoveSceneListeners, true);
        }

        public T Get<T>() where T : ISignal, new()
        {
            Type signalType = typeof(T);
            if (!signals.ContainsKey(signalType)) signals.Add(signalType, new T());
            return (T)signals[signalType];
        }

        public void RemoveSceneListeners()
        {
            foreach (var listener in signals.Values)
            {
                listener.RemoveAllSceneListeners();
            }
        }
    }
}

public abstract class Signal : ISignal
{
    private Action sceneCallback;
    private Action globalCallback;

    public void AddSceneListener(Action handler) => sceneCallback += handler;

    public void AddGlobalListener(Action handler) => globalCallback += handler;

    public void RemoveSceneListener(Action handler) => sceneCallback -= handler;

    public void RemoveGlobalListener(Action handler) => globalCallback -= handler;

    public void RemoveAllSceneListeners() => sceneCallback = null;

    public void RemoveAllGlobalListeners() => globalCallback = null;

    public void RemoveAllListeners()
    {
        sceneCallback = null;
        globalCallback = null;
    }

    public void Dispatch()
    {
        sceneCallback?.Invoke();
        globalCallback?.Invoke();
    }
}

public abstract class Signal<T> : ISignal
{
    private Action<T> sceneCallback;
    private Action<T> globalCallback;

    public void AddSceneListener(Action<T> handler) => sceneCallback += handler;

    public void AddGlobalListener(Action<T> handler) => globalCallback += handler;

    public void RemoveSceneListener(Action<T> handler) => sceneCallback -= handler;

    public void RemoveGlobalListener(Action<T> handler) => globalCallback -= handler;

    public void RemoveAllSceneListeners() => sceneCallback = null;

    public void RemoveAllGlobalListeners() => globalCallback = null;

    public void RemoveAllListeners()
    {
        sceneCallback = null;
        globalCallback = null;
    }

    public void Dispatch(T arg)
    {
        sceneCallback?.Invoke(arg);
        globalCallback?.Invoke(arg);
    }
}

public abstract class Signal<T1, T2> : ISignal
{
    private Action<T1, T2> sceneCallback;
    private Action<T1, T2> globalCallback;

    public void AddSceneListener(Action<T1, T2> handler) => sceneCallback += handler;

    public void AddGlobalListener(Action<T1, T2> handler) => globalCallback += handler;

    public void RemoveSceneListener(Action<T1, T2> handler) => sceneCallback -= handler;

    public void RemoveGlobalListener(Action<T1, T2> handler) => globalCallback -= handler;

    public void RemoveAllSceneListeners() => sceneCallback = null;

    public void RemoveAllGlobalListeners() => globalCallback = null;

    public void RemoveAllListeners()
    {
        sceneCallback = null;
        globalCallback = null;
    }

    public void Dispatch(T1 arg1, T2 arg2)
    {
        sceneCallback?.Invoke(arg1, arg2);
        globalCallback?.Invoke(arg1, arg2);
    }
}
