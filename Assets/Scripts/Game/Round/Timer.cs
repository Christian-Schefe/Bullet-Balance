using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Timer
{
    public System.Action<float> action;
    public abstract void Tick(float time);
}

public class SimpleTimer : Timer
{
    public float duration;
    public float timeOffset;

    public SimpleTimer(float duration)
    {
        this.duration = duration;
        timeOffset = 0;
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
        currentDuration = Random.Range(minInitialWait, Mathf.Max(minInitialWait, maxDuration));
    }

    public override void Tick(float time)
    {
        var current = time - timeOffset;
        if (current >= currentDuration)
        {
            action?.Invoke(time);
            timeOffset += currentDuration;
            currentDuration = Random.Range(minDuration, maxDuration);
        }
    }
}