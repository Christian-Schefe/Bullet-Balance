using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveAsset", menuName = "Waves/WaveAsset")]
public class WaveAsset : ScriptableObject
{
    [SerializeField] private List<InitialEnemyData> initialEnemies;
    [SerializeField] private List<DelayedEnemyData> delayedEnemies;

    public EnemyWave CreateWave()
    {
        var initials = new Dictionary<int, EnemyObject>();
        foreach (var data in initialEnemies)
        {
            initials.Add(data.slot, data.enemy);
        }
        var delayed = new List<(float, EnemyObject)>();
        foreach (var data in delayedEnemies)
        {
            delayed.Add((data.delay, data.enemy));
        }
        var wave = new EnemyWave(initials, delayed);
        return wave;
    }

    [System.Serializable]
    public struct InitialEnemyData
    {
        public int slot;
        public EnemyObject enemy;
    }

    [System.Serializable]
    public struct DelayedEnemyData
    {
        public float delay;
        public EnemyObject enemy;
    }
}
