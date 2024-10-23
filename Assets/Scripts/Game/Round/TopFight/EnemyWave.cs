using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave
{
    private Dictionary<int, EnemyType> initialWave;
    private Queue<(float, EnemyType)> wave;

    public static EnemyWave TestWave1 => new(
        new() { { 4, EnemyType.Knight }, { 5, EnemyType.Knight } },
        new() { (0, EnemyType.Knight), (7, EnemyType.Archer), (14, EnemyType.Knight) }
    );
    public static EnemyWave EliteWave1 => new(
        new() { { 3, EnemyType.Knight }, { 4, EnemyType.Knight }, { 5, EnemyType.Archer } },
        new() { (0, EnemyType.Knight), (0, EnemyType.Knight), (0, EnemyType.Knight), (0, EnemyType.Archer) }
    );

    public EnemyWave(Dictionary<int, EnemyType> initialWave, List<(float, EnemyType)> spawns)
    {
        wave = new(spawns);
        this.initialWave = initialWave;
    }

    public bool IsEmpty => wave.Count == 0;

    public EnemyType? GetInitialEnemy(int position)
    {
        return initialWave.TryGetValue(position, out var type) ? type : null;
    }

    public bool TrySpawn(float time, out EnemyType enemyType)
    {
        if (IsEmpty)
        {
            enemyType = default;
            return false;
        }

        var (spawnTime, type) = wave.Peek();
        if (time >= spawnTime)
        {
            wave.Dequeue();
            enemyType = type;
            return true;
        }

        enemyType = default;
        return false;
    }
}

public enum EnemyType
{
    Knight, Archer
}