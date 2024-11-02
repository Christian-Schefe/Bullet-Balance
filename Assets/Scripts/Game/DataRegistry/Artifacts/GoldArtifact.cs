using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoldArtifact", menuName = "Artifacts/GoldArtifact")]
public class GoldArtifact : ArtifactObject
{
    [SerializeField] private float artifactPriceFactor;
    [SerializeField] private float hazardPriceFactor;
    [SerializeField] private int afterRoundGold;
    [SerializeField] private int afterKillGold;
    [SerializeField] private float goldSpawnRateFactor;

    public override void OnAquire()
    {
        DataManager.StatsData.ArtifactPrice *= artifactPriceFactor;
        DataManager.StatsData.HazardPrice *= hazardPriceFactor;
        DataManager.StatsData.GoldSpawnRate *= goldSpawnRateFactor;
    }

    public override void OnFinishRound()
    {
        DataManager.AddGold(afterRoundGold);
    }

    public override void OnKillEnemy(Enemy enemy)
    {
        DataManager.AddGold(afterKillGold);
    }
}
