using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemySpawnerEntry[] enemyPrefabs;

    private Dictionary<EnemyType, Enemy> enemyPrefabsDict;

    private void Awake()
    {
        enemyPrefabsDict = new Dictionary<EnemyType, Enemy>();
        foreach (var entry in enemyPrefabs)
        {
            enemyPrefabsDict[entry.type] = entry.prefab;
        }
    }

    public Enemy SpawnEnemy(EnemyType type, Transform parent)
    {
        if (!enemyPrefabsDict.ContainsKey(type))
        {
            Debug.LogError($"No prefab found for enemy type {type}");
            return null;
        }

        return Instantiate(enemyPrefabsDict[type], parent);
    }
}

[Serializable]
public class EnemySpawnerEntry
{
    public EnemyType type;
    public Enemy prefab;
}