using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yeast;

public class MapData
{
    public readonly PersistentValue<WorldList> worldListStore = new("map.worldList", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<Vector2Int> playerPositionStore = new("map.playerPosition", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> worldIndexStore = new("map.worldIndex", PersistenceMode.GlobalPersistence);

    public readonly PersistentValue<CurrentNodeInfo> currentNodeInfoStore = new("map.currentNodeInfo", PersistenceMode.GlobalRuntime);

    public WorldList WorldList
    {
        get => worldListStore.Get();
        set => worldListStore.Set(value);
    }

    public Vector2Int PlayerPosition
    {
        get => playerPositionStore.Get();
        set => playerPositionStore.Set(value);
    }

    public int WorldIndex
    {
        get => worldIndexStore.Get();
        set => worldIndexStore.Set(value);
    }

    public CurrentNodeInfo CurrentNodeInfo
    {
        get => currentNodeInfoStore.GetOrDefault(new() { nodeType = NodeType.Fight });
        set => currentNodeInfoStore.Set(value);
    }

    public void Reset()
    {
        worldListStore.Set(new());
        playerPositionStore.Set(Vector2Int.zero);
        worldIndexStore.Set(0);
        currentNodeInfoStore.Remove();
    }

    public void CreateSnapshot(Dictionary<string, string> snapshot)
    {
        snapshot[playerPositionStore.Key] = PlayerPosition.ToJson();
        snapshot[worldIndexStore.Key] = WorldIndex.ToJson();
    }

    public void ApplySnapshot(Dictionary<string, string> snapshot)
    {
        PlayerPosition = snapshot[playerPositionStore.Key].FromJson<Vector2Int>();
        WorldIndex = snapshot[worldIndexStore.Key].FromJson<int>();
    }
}

public class RunData
{
    public readonly PersistentValue<RunState> runStateStore = new("run.runState", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> seedStore = new("run.Seed", PersistenceMode.GlobalPersistence);

    public RunState RunState
    {
        get => runStateStore.GetOrDefault(RunState.GameOver);
        set => runStateStore.Set(value);
    }

    public int Seed
    {
        get => seedStore.Get();
        set => seedStore.Set(value);
    }

    public void Reset(int seed)
    {
        seedStore.Set(seed);
        runStateStore.Set(RunState.Running);
    }
}

public enum RunState
{
    Running, GameOver, Win
}

public class SettingsData
{
    public readonly PersistentValue<float> musicVolumeStore = new("settings.musicVolume", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<float> sfxVolumeStore = new("settings.sfxVolume", PersistenceMode.GlobalPersistence);

    public float MusicVolume
    {
        get => musicVolumeStore.GetOrDefault(1);
        set => musicVolumeStore.Set(value);
    }

    public float SfxVolume
    {
        get => sfxVolumeStore.GetOrDefault(1);
        set => sfxVolumeStore.Set(value);
    }

    public void Reset()
    {
        musicVolumeStore.Remove();
        sfxVolumeStore.Remove();
    }
}

public class PlayerData
{
    public readonly PersistentValue<int> healthStore = new("player.health", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> maxHealthStore = new("player.maxHealth", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> goldStore = new("player.gold", PersistenceMode.GlobalPersistence);

    public int Health
    {
        get => healthStore.Get();
        set => healthStore.Set(value);
    }

    public int MaxHealth
    {
        get => maxHealthStore.Get();
        set => maxHealthStore.Set(value);
    }

    public int Gold
    {
        get => goldStore.Get();
        set => goldStore.Set(value);
    }

    public void HealBy(int amount)
    {
        Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    }

    public void DamageBy(int amount)
    {
        Health = Mathf.Clamp(Health - amount, 0, MaxHealth);
    }

    public void Reset(int startHealth, int startGold)
    {
        Health = startHealth;
        MaxHealth = startHealth;
        Gold = startGold;
    }

    public void CreateSnapshot(Dictionary<string, string> snapshot)
    {
        snapshot[healthStore.Key] = Health.ToJson();
        snapshot[maxHealthStore.Key] = MaxHealth.ToJson();
        snapshot[goldStore.Key] = Gold.ToJson();
    }

    public void ApplySnapshot(Dictionary<string, string> snapshot)
    {
        Health = snapshot[healthStore.Key].FromJson<int>();
        MaxHealth = snapshot[maxHealthStore.Key].FromJson<int>();
        Gold = snapshot[goldStore.Key].FromJson<int>();
    }
}

public class InventoryData
{
    public readonly PersistentValue<List<(string, int)>> hazardLevelsStore = new("hazardLevelsStore", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<HashSet<string>> artifactsStore = new("artifactsStore", PersistenceMode.GlobalPersistence);

    public List<(string, int)> HazardLevels => hazardLevelsStore.GetOrSetDefault(new());
    public HashSet<string> Artifacts => artifactsStore.GetOrSetDefault(new());

    public void AddArtifact(string artifactId)
    {
        bool added = Artifacts.Add(artifactId);
        artifactsStore.MarkDirty();

        if (added)
        {
            Signals.Get<InventoryOnAquireArtifact>().Dispatch(artifactId);
        }
    }

    public void AddHazard(string hazardId, int level)
    {
        HazardLevels.Add((hazardId, level));
        hazardLevelsStore.MarkDirty();
    }

    public void SetLevel(int index, int level)
    {
        HazardLevels[index] = (HazardLevels[index].Item1, level);
        hazardLevelsStore.MarkDirty();
    }

    public void Reset()
    {
        hazardLevelsStore.Set(new());
        artifactsStore.Set(new());
    }

    public void CreateSnapshot(Dictionary<string, string> snapshot)
    {
        snapshot[hazardLevelsStore.Key] = HazardLevels.ToJson();
        snapshot[artifactsStore.Key] = Artifacts.ToJson();
    }

    public void ApplySnapshot(Dictionary<string, string> snapshot)
    {
        hazardLevelsStore.Set(snapshot[hazardLevelsStore.Key].FromJson<List<(string, int)>>());
        artifactsStore.Set(snapshot[artifactsStore.Key].FromJson<HashSet<string>>());
    }
}
