using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveCollection", menuName = "Waves/WaveCollection")]
public class WaveCollection : ScriptableObject
{
    public List<WaveData> waves;

    public EnemyWave GetRandomWave(float difficulty)
    {
        var possibleWaves = new List<WaveAsset>();
        foreach (var wave in waves)
        {
            if (wave.difficultyRange.x <= difficulty && wave.difficultyRange.y >= difficulty)
            {
                possibleWaves.Add(wave.wave);
            }
        }
        if (possibleWaves.Count == 0)
        {
            Debug.LogWarning("No waves found for difficulty " + difficulty);
            return waves[Random.Range(0, waves.Count)].wave.CreateWave();
        }
        return possibleWaves[Random.Range(0, possibleWaves.Count)].CreateWave();
    }

    [System.Serializable]
    public struct WaveData
    {
        public WaveAsset wave;
        public Vector2 difficultyRange;
    }
}
