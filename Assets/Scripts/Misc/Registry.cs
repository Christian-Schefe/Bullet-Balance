using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry<TKey, TValue> : ScriptableObject where TValue : IRegistryEntry<TKey>
{
    [SerializeField] private List<TValue> data;

    private Dictionary<TKey, TValue> lookup;

    public Dictionary<TKey, TValue> Lookup => GetLookup();

    private Dictionary<TKey, TValue> GetLookup()
    {
        if (lookup == null)
        {
            lookup = new();
            foreach (var data in data)
            {
                var key = data.Key;
                lookup[key] = data;
            }
        }
        return lookup;
    }

    public TValue this[TKey index] => Lookup[index];
}

public interface IRegistryEntry<TKey>
{
    TKey Key { get; }
}
