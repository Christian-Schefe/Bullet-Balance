using System.Collections.Generic;

public class EnemyWave
{
    private Dictionary<int, EnemyObject> initialWave;
    private Queue<(float, EnemyObject)> wave;

    public EnemyWave(Dictionary<int, EnemyObject> initialWave, List<(float, EnemyObject)> spawns)
    {
        wave = new(spawns);
        this.initialWave = initialWave;
    }

    public bool IsEmpty => wave.Count == 0;

    public EnemyObject GetInitialEnemy(int position)
    {
        return initialWave.TryGetValue(position, out var type) ? type : null;
    }

    public bool TrySpawn(float time, out EnemyObject enemyType)
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
