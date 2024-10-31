using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeededRandom
{
    private int Seed { get; }
    private readonly System.Random random;

    public SeededRandom(int seed)
    {
        Seed = seed;
        random = new System.Random(Seed);
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
        return IntRange(min, max + 1);
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

    public int GenSeed()
    {
        return random.Next(int.MinValue, int.MaxValue);
    }

    public void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            var temp = list[i];
            var randomIndex = IntRange(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
