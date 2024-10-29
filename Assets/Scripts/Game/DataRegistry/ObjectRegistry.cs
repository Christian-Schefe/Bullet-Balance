using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectRegistry<T> : ScriptableObject where T : ScriptableObject, IRegistryObject
{
    [SerializeField] private List<T> objects = new();

    private Dictionary<string, T> objectsById;
    public IEnumerable<T> Enumerator => objects;

    private void GenerateDictionary()
    {
        objectsById = new Dictionary<string, T>();
        foreach (var hazard in objects)
        {
            objectsById[hazard.Id] = hazard;
        }
    }

    public T Lookup(string id)
    {
        if (objectsById == null)
        {
            GenerateDictionary();
        }

        return objectsById[id];
    }
}

public interface IRegistryObject
{
    string Id { get; }
}
