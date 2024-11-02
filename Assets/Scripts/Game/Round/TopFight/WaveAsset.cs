using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveAsset", menuName = "Waves/WaveAsset")]
public class WaveAsset : ScriptableObject
{
    [SerializeField] private List<EnemyObject> initialEnemies;
    [SerializeField] private List<DelayedEnemyData> delayedEnemies;

    public EnemyWave CreateWave()
    {
        var initials = new Dictionary<int, EnemyObject>();
        for(int i = 0; i < initialEnemies.Count; i++)
        {
            initials.Add(i, initialEnemies[i]);
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
    public struct DelayedEnemyData
    {
        public float delay;
        public EnemyObject enemy;
    }
}
