using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGenerator : MonoBehaviour
{
    [SerializeField] private List<WaveCollection> normalCollections;
    [SerializeField] private List<WaveCollection> eliteCollections;
    [SerializeField] private List<WaveCollection> bossCollections;

    public EnemyWave GenerateWave(WaveType waveType)
    {
        int worldIndex = DataManger.MapData.WorldIndex;
        float worldProgress = DataManger.MapData.CurrentNodeInfo.difficulty;

        var collections = waveType switch
        {
            WaveType.Normal => normalCollections,
            WaveType.Elite => eliteCollections,
            WaveType.Boss => bossCollections,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        var collection = collections[Mathf.Min(worldIndex, collections.Count - 1)];

        var rng = new SeededRandom(DataManger.MapData.CurrentNodeInfo.sceneSeed);
        return collection.GetRandomWave(rng, worldProgress);
    }
}

public enum WaveType
{
    Normal,
    Elite,
    Boss
}