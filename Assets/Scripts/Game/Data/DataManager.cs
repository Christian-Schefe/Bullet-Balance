using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private int startGold;

    private readonly PersistentValue<Dictionary<string, string>> snapshotStore = new("snapshot", PersistenceMode.GlobalPersistence);

    private RunData runData;
    private PlayerData playerData;
    private InventoryData inventoryData;
    private SettingsData settingsData;
    private MapData mapData;
    private StatsData statsData;

    public static RunData RunData => Globals<DataManager>.Instance.GetRunData();
    public static PlayerData PlayerData => Globals<DataManager>.Instance.GetPlayerData();
    public static InventoryData InventoryData => Globals<DataManager>.Instance.GetInventoryData();
    public static SettingsData SettingsData => Globals<DataManager>.Instance.GetSettingsData();
    public static MapData MapData => Globals<DataManager>.Instance.GetMapData();
    public static StatsData StatsData => Globals<DataManager>.Instance.GetStatsData();

    private static ISnapshotable[] Snapshotables => new ISnapshotable[] { MapData, PlayerData, InventoryData, StatsData };

    public RunData GetRunData()
    {
        runData ??= new RunData();
        return runData;
    }

    public PlayerData GetPlayerData()
    {
        playerData ??= new PlayerData();
        return playerData;
    }

    public InventoryData GetInventoryData()
    {
        inventoryData ??= new InventoryData();
        return inventoryData;
    }

    public SettingsData GetSettingsData()
    {
        settingsData ??= new SettingsData();
        return settingsData;
    }

    public MapData GetMapData()
    {
        mapData ??= new MapData();
        return mapData;
    }

    public StatsData GetStatsData()
    {
        statsData ??= new StatsData();
        return statsData;
    }

    public bool CanContinueRun()
    {
        return snapshotStore.TryGet(out _) && RunData.RunState == RunState.Running;
    }

    public void CreateSnapshot()
    {
        var data = new Dictionary<string, string>();
        foreach (var snapshotable in Snapshotables)
        {
            snapshotable.CreateSnapshot(data);
        }
        snapshotStore.Set(data);
    }

    public void ContinueRun()
    {
        var data = snapshotStore.Get();
        foreach (var snapshotable in Snapshotables)
        {
            snapshotable.ApplySnapshot(data);
        }
    }

    public void StartNewRun()
    {
        var seed = Random.Range(int.MinValue, int.MaxValue);
        RunData.Reset(seed);
        MapData.Reset();
        PlayerData.Reset(startHealth, startGold);
        InventoryData.Reset();
        StatsData.Reset();

        InventoryData.AddHazard("cannon", 0);

        print($"Starting new run with Seed {seed}");
    }

    public static void AddGold(int amount)
    {
        PlayerData.Gold += amount;
    }

    public static bool TrySpendGold(int amount)
    {
        if (PlayerData.Gold < amount) return false;
        PlayerData.Gold -= amount;
        return true;
    }

    public static void HealPlayer(int amount)
    {
        PlayerData.HealBy(amount);
    }

    public static bool DamagePlayer(int amount)
    {
        PlayerData.DamageBy(amount);
        if (PlayerData.Health == 0)
        {
            Globals<RunManager>.Instance.GameOver();
            return true;
        }
        return false;
    }

    public static void IncreaseMaxHealth(int amount)
    {
        PlayerData.MaxHealth += amount;
        PlayerData.HealBy(amount);
    }

    public static void DecreaseMaxHealth(int amount)
    {
        PlayerData.MaxHealth = Mathf.Max(PlayerData.MaxHealth - amount, 1);
        if (PlayerData.Health > PlayerData.MaxHealth) PlayerData.Health = PlayerData.MaxHealth;
    }
}

public interface ISnapshotable
{
    void CreateSnapshot(Dictionary<string, string> snapshot);
    void ApplySnapshot(Dictionary<string, string> snapshot);
}
