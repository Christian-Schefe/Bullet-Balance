using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class Arena : MonoBehaviour
{
    [SerializeField] private Transform minCorner, maxCorner, bulletContainer;

    [SerializeField] private Player player;
    [SerializeField] private TopFight topFight;
    [SerializeField] private HazardRegistry hazardRegistry;
    [SerializeField] private GoldSpawner goldSpawner;

    private RunManager runManager;
    private InventoryData inventoryData;

    private readonly List<ITickable> tickables = new();

    public Vector2 MinCorner => minCorner.position;
    public Vector2 MaxCorner => maxCorner.position;
    public float Width => maxCorner.position.x - minCorner.position.x;
    public float Height => maxCorner.position.y - minCorner.position.y;
    public Player Player => player;
    public TopFight TopFight => topFight;
    public float GameTime => time;

    private float time;
    private bool isDone;
    private bool isPaused;

    private bool isPauseMenuOpen, isInventoryOpen;

    private readonly List<ITickable> tickablesToRemove = new();

    private void Awake()
    {
        time = 0;
        runManager = Globals<RunManager>.Instance;
        inventoryData = DataManager.InventoryData;
        isDone = false;
        isPaused = false;

        Signals.Get<PauseMenuIsOpen>().AddSceneListener(OnPauseMenuIsOpenChanged);
        Signals.Get<InventoryIsOpen>().AddSceneListener(OnInventoryIsOpenChanged);

        tickables.Add(player);
        tickables.Add(goldSpawner);

        SpawnHazards();
    }

    private void Update()
    {
        if (isDone || isPaused) return;
        time += Time.deltaTime;

        foreach (ITickable tickable in tickablesToRemove)
        {
            tickables.Remove(tickable);
        }
        tickablesToRemove.Clear();

        foreach (ITickable tickable in tickables)
        {
            tickable.Tick(time);
        }

        topFight.Tick(time, out var hasPlayerWon);

        if (hasPlayerWon)
        {
            if (DataManager.MapData.CurrentNodeInfo.nodeType == NodeType.Boss)
            {
                DataManager.PlayerData.HealBy(DataManager.PlayerData.MaxHealth);
            }
            Signals.Get<ArenaOnWinFight>().Dispatch();
            runManager.LoadScene(SceneType.Reward);
            isDone = true;
        }
    }

    private void OnPauseMenuIsOpenChanged(bool isOpen)
    {
        isPauseMenuOpen = isOpen;
        isPaused = isPauseMenuOpen || isInventoryOpen;
    }

    private void OnInventoryIsOpenChanged(bool isOpen)
    {
        isInventoryOpen = isOpen;
        isPaused = isPauseMenuOpen || isInventoryOpen;
    }

    private void SpawnHazards()
    {
        var hazardList = DataManager.InventoryData.HazardLevels;
        foreach (var (hazardId, level) in hazardList)
        {
            var hazard = hazardRegistry.Lookup(hazardId).CreateHazard();
            tickables.Add(hazard);
            hazard.Init(this, level);
        }
    }

    public void CollectGold(int amount)
    {
        DataManager.AddGold(amount);
    }

    public void DealDamage(int damage)
    {
        if (DataManager.DamagePlayer(damage))
        {
            isDone = true;
        }
    }

    public T CreateBulletObject<T>(T prefab) where T : MonoBehaviour
    {
        var instance = Instantiate(prefab, bulletContainer);
        return instance;
    }

    public void AddTickable(ITickable tickable)
    {
        tickables.Add(tickable);
    }

    public void ScheduleRemoveTickable(ITickable tickable)
    {
        tickablesToRemove.Add(tickable);
    }

    public Vector2 Constrain(Vector2 pos, float radius)
    {
        return new Vector2(
            Mathf.Clamp(pos.x, minCorner.position.x + radius, maxCorner.position.x - radius),
            Mathf.Clamp(pos.y, minCorner.position.y + radius, maxCorner.position.y - radius)
        );
    }

    public bool IsFullyOutside(Vector2 pos, float radius)
    {
        return pos.x + radius < minCorner.position.x ||
               pos.x - radius > maxCorner.position.x ||
               pos.y + radius < minCorner.position.y ||
               pos.y - radius > maxCorner.position.y;
    }

    public Vector2[] RandomEdgePosition(params float[] edgeOffsets)
    {
        var results = new Vector2[edgeOffsets.Length];
        var circumference = 2 * (Width + Height);
        var random = Random.Range(0, circumference);
        if (random < Width)
        {
            for (int i = 0; i < edgeOffsets.Length; i++)
            {
                var edgeOffset = edgeOffsets[i];
                results[i] = new Vector2(minCorner.position.x + random, minCorner.position.y - edgeOffset);
            }
            return results;
        }
        random -= Width;
        if (random < Width)
        {
            for (int i = 0; i < edgeOffsets.Length; i++)
            {
                var edgeOffset = edgeOffsets[i];
                results[i] = new Vector2(minCorner.position.x + random, maxCorner.position.y + edgeOffset);
            }
            return results;
        }
        random -= Width;
        if (random < Height)
        {
            for (int i = 0; i < edgeOffsets.Length; i++)
            {
                var edgeOffset = edgeOffsets[i];
                results[i] = new Vector2(minCorner.position.x - edgeOffset, minCorner.position.y + random);
            }
            return results;
        }
        random -= Height;
        for (int i = 0; i < edgeOffsets.Length; i++)
        {
            var edgeOffset = edgeOffsets[i];
            results[i] = new Vector2(maxCorner.position.x + edgeOffset, minCorner.position.y + random);
        }
        return results;
    }

    public Vector2 RandomPosition(float radius)
    {
        var x = Random.Range(minCorner.position.x + radius, maxCorner.position.x - radius);
        var y = Random.Range(minCorner.position.y + radius, maxCorner.position.y - radius);
        return new Vector2(x, y);
    }
}
