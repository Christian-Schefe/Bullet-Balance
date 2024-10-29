using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeededRandom
{
    private readonly System.Random random;

    public SeededRandom(int? seed)
    {
        if (seed.HasValue) random = new System.Random(seed.Value);
        else random = new System.Random();
    }

    public bool Probability(float probability)
    {
        return random.NextDouble() < probability;
    }

    public float Range(float min, float max)
    {
        return (float)random.NextDouble() * (max - min) + min;
    }

    public int IntRange(int min, int max)
    {
        return (int)(random.NextDouble() * (max - min) + min);
    }

    public int IntRangeInclusive(int min, int max)
    {
        return (int)(random.NextDouble() * (max - min + 1) + min);
    }

    public Vector2 InsideUnitCircle()
    {
        var angle = Range(0f, 2 * Mathf.PI);
        var radius = Mathf.Sqrt(Range(0f, 1f));
        return new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
    }

    public T Choose<T>(IReadOnlyList<T> list)
    {
        return list[IntRange(0, list.Count)];
    }
}
