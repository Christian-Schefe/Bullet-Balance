using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public readonly PersistentValue<WorldList> worldListStore = new("map.worldList", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<Vector2Int> playerPositionStore = new("map.playerPosition", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> worldIndexStore = new("map.worldIndex", PersistenceMode.GlobalPersistence);

    public readonly PersistentValue<float> currentDifficultyStore = new("map.difficulty", PersistenceMode.GlobalRuntime);
    public readonly PersistentValue<NodeType> currentNodeTypeStore = new("map.nodeType", PersistenceMode.GlobalRuntime);

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

    public float CurrentDifficulty
    {
        get => currentDifficultyStore.GetOrDefault(0);
        set => currentDifficultyStore.Set(value);
    }

    public NodeType CurrentNodeType
    {
        get => currentNodeTypeStore.GetOrDefault(NodeType.Fight);
        set => currentNodeTypeStore.Set(value);
    }

    public void Reset()
    {
        worldListStore.Set(new());
        playerPositionStore.Set(Vector2Int.zero);
        worldIndexStore.Set(0);
        currentDifficultyStore.Remove();
        currentNodeTypeStore.Remove();
    }
}

public class RunData
{
    public readonly PersistentValue<bool> isRunningStore = new("run.isRunning", PersistenceMode.GlobalPersistence);
    public readonly PersistentValue<int> seedStore = new("run.seed", PersistenceMode.GlobalPersistence);

    public bool IsRunning
    {
        get => isRunningStore.GetOrDefault(false);
        set => isRunningStore.Set(value);
    }

    public int Seed
    {
        get => seedStore.Get();
        set => seedStore.Set(value);
    }

    public void Reset(int seed)
    {
        seedStore.Set(seed);
        isRunningStore.Set(true);
    }
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
        Health = Mathf.Min(MaxHealth, Health + amount);
    }

    public void DamageBy(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
    }

    public void Reset(int startHealth, int startGold)
    {
        Health = startHealth;
        MaxHealth = startHealth;
        Gold = startGold;
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
}
