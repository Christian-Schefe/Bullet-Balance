using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactApplier : MonoBehaviour
{
    [SerializeField] private ArtifactRegistry artifactRegistry;

    private void Awake()
    {
        print("ArtifactApplier.Awake");

        Signals.Get<ArenaOnWinFight>().AddSceneListener(OnWinFight);
        Signals.Get<ArenaOnKillEnemy>().AddSceneListener(OnKillEnemy);
        Signals.Get<InventoryOnAquireArtifact>().AddSceneListener(OnAquireArtifact);
    }

    private void OnWinFight()
    {
        print("ArtifactApplier.OnWinFight");
        ForEachArtifact(obj => obj.OnFinishRound());
    }

    private void OnKillEnemy(Enemy enemy)
    {
        print("ArtifactApplier.OnKillEnemy");
        ForEachArtifact(obj => obj.OnKillEnemy(enemy));
    }

    private void OnAquireArtifact(string id)
    {
        print("ArtifactApplier.OnAquireArtifact");
        var artifact = artifactRegistry.Lookup(id);
        artifact.OnAquire();
    }

    public int CalculateArtifactPrice(int basePrice)
    {
        print("ArtifactApplier.CalculateArtifactPrice");
        return CumulateForEachArtifact((artifact, price) => artifact.CalculateArtifactPrice(price), basePrice);
    }

    public int CalculateHazardPrice(int basePrice)
    {
        print("ArtifactApplier.CalculateHazardPrice");
        return CumulateForEachArtifact((artifact, price) => artifact.CalculateHazardPrice(price), basePrice);
    }

    public int CalculateHealPrice(int basePrice)
    {
        print("ArtifactApplier.CalculateHealPrice");
        return CumulateForEachArtifact((artifact, price) => artifact.CalculateHealPrice(price), basePrice);
    }

    public int CalculateHealAmount(int baseAmount)
    {
        print("ArtifactApplier.CalculateHealAmount");
        return CumulateForEachArtifact((artifact, price) => artifact.CalculateHealAmount(price), baseAmount);
    }

    private void ForEachArtifact(Action<ArtifactObject> action)
    {
        var artifacts = DataManger.InventoryData.Artifacts;
        foreach (var id in artifacts)
        {
            var artifact = artifactRegistry.Lookup(id);
            action(artifact);
        }
    }

    private T CumulateForEachArtifact<T>(Func<ArtifactObject, T, T> action, T initial)
    {
        var artifacts = DataManger.InventoryData.Artifacts;
        T val = initial;
        foreach (var id in artifacts)
        {
            var artifact = artifactRegistry.Lookup(id);
            val = action(artifact, val);
        }
        return val;
    }
}
