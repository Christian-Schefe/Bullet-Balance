using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class GoldSpawner : MonoBehaviour, ITickable
{
    [SerializeField] private Arena arena;
    [SerializeField] private GoldEntity goldPrefab;

    [SerializeField] private float goldRadius;

    private Timer timer;

    private void Awake()
    {
        var spawnRate = DataManager.StatsData.GoldSpawnRate;
        timer = new SimpleTimer(1.0f / spawnRate, (_) => SpawnGold());
    }

    public void Tick(float time)
    {
        timer.Tick(time);
    }

    private void SpawnGold()
    {
        var pos = arena.RandomPosition(goldRadius);
        var gold = Instantiate(goldPrefab, pos, Quaternion.identity);
        gold.arena = arena;
        arena.ScheduleAddTickable(gold);
        gold.Spawn();
    }
}
