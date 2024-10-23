using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManger : MonoBehaviour
{
    [SerializeField] private int startHealth;
    [SerializeField] private int startGold;

    private RunData runData;
    private PlayerData playerData;
    private InventoryData inventoryData;
    private SettingsData settingsData;
    private MapData mapData;

    public static RunData RunData => Globals<DataManger>.Instance.GetRunData();
    public static PlayerData PlayerData => Globals<DataManger>.Instance.GetPlayerData();
    public static InventoryData InventoryData => Globals<DataManger>.Instance.GetInventoryData();
    public static SettingsData SettingsData => Globals<DataManger>.Instance.GetSettingsData();
    public static MapData MapData => Globals<DataManger>.Instance.GetMapData();

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

    public void StartNewRun()
    {
        var seed = Random.Range(int.MinValue, int.MaxValue);
        RunData.Reset(seed);
        MapData.Reset();
        PlayerData.Reset(startHealth, startGold);
        InventoryData.Reset();

        InventoryData.AddHazard("cannon", 0);

        print($"Starting new run with seed {seed}");
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
        return PlayerData.Health == 0;
    }

    public static void IncreaseMaxHealth(int amount)
    {
        PlayerData.MaxHealth += amount;
        PlayerData.HealBy(amount);
    }
}
