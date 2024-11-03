using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer
{
    public Action<float> action;
    public abstract void Tick(float time);
}

public class SimpleTimer : Timer
{
    public float duration;
    public float timeOffset;

    public SimpleTimer(float duration, Action<float> action = null)
    {
        this.duration = duration;
        timeOffset = 0;
        this.action = action;
    }

    public override void Tick(float time)
    {
        var current = time - timeOffset;
        if (current >= duration)
        {
            action?.Invoke(time);
            timeOffset += duration;
        }
    }
}

public class RandomTimer : Timer
{
    public float minDuration;
    public float maxDuration;
    public float timeOffset;
    private float currentDuration;

    public RandomTimer(float minDuration, float maxDuration, float minInitialWait)
    {
        this.minDuration = minDuration;
        this.maxDuration = maxDuration;
        timeOffset = 0;
        currentDuration = UnityEngine.Random.Range(minInitialWait, Mathf.Max(minInitialWait, maxDuration));
    }

    public override void Tick(float time)
    {
        var current = time - timeOffset;
        if (current >= currentDuration)
        {
            action?.Invoke(time);
            timeOffset += currentDuration;
            currentDuration = UnityEngine.Random.Range(minDuration, maxDuration);
        }
    }
}