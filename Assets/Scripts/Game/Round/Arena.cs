using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] private Transform minCorner, maxCorner, bulletContainer;

    [SerializeField] private Player player;
    [SerializeField] private TopFight topFight;
    [SerializeField] private HazardRegistry hazardRegistry;
    [SerializeField] private GoldSpawner goldSpawner;

    private RunManager runManager;
    private PlayerData playerData;
    private InventoryData inventoryData;

    private List<IHazard> hazards = new();
    private List<Projectile> bullets = new();

    public Vector2 MinCorner => minCorner.position;
    public Vector2 MaxCorner => maxCorner.position;
    public float Width => maxCorner.position.x - minCorner.position.x;
    public float Height => maxCorner.position.y - minCorner.position.y;
    public Player Player => player;
    public float GameTime => time;

    private float time;
    private bool isDone;
    private bool isPaused;

    private bool isPauseMenuOpen, isInventoryOpen;

    private void Awake()
    {
        time = 0;
        runManager = Globals<RunManager>.Instance;
        playerData = DataManger.PlayerData;
        inventoryData = DataManger.InventoryData;
        isDone = false;
        isPaused = false;

        Signals.Get<PauseMenuIsOpen>().AddSceneListener(OnPauseMenuIsOpenChanged);
        Signals.Get<InventoryIsOpen>().AddSceneListener(OnInventoryIsOpenChanged);

        SpawnHazards();

    }

    private void Update()
    {
        if (isDone || isPaused) return;
        time += Time.deltaTime;

        player.Tick(time);
        goldSpawner.Tick(time);

        foreach (IHazard hazard in hazards)
        {
            hazard.Tick(time);
        }

        var bulletsToRemove = new List<Projectile>();

        foreach (Projectile bullet in bullets)
        {
            bullet.Tick(time, out var shouldDestroy);
            if (shouldDestroy)
            {
                bulletsToRemove.Add(bullet);
                bullet.DealEnemyDamage(topFight);
                bullet.HandleDestroy(false);
            }
            else if (bullet.IsHit(player))
            {
                bulletsToRemove.Add(bullet);
                bullet.DealPlayerDamage(topFight);
                bullet.HandleDestroy(true);
            }
        }

        foreach (Projectile bullet in bulletsToRemove)
        {
            bullets.Remove(bullet);
        }

        topFight.Tick(time, out var hasPlayerWon);

        if (hasPlayerWon)
        {
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
        var hazardList = inventoryData.HazardLevels;
        foreach (var (hazardId, level) in hazardList)
        {
            print("Spawning hazardId " + hazardId + " level " + level);
            var hazard = hazardRegistry.Lookup(hazardId).CreateHazard();
            hazards.Add(hazard);
            hazard.Init(this, level);
        }
    }

    public void CollectGold(int amount)
    {
        DataManger.AddGold(amount);
    }

    public void DealDamage(int damage)
    {
        playerData.Health = Mathf.Max(0, playerData.Health - damage);
        if (playerData.Health == 0)
        {
            runManager.GameOver();
            isDone = true;
        }
    }

    public T CreateBulletObject<T>(T prefab) where T : MonoBehaviour
    {
        var instance = Instantiate(prefab, bulletContainer);
        return instance;
    }

    public void AddBullet(Projectile bullet)
    {
        bullets.Add(bullet);
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
