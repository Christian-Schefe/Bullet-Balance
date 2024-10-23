using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthArtifact", menuName = "Artifacts/HealthArtifact")]
public class GoldArtifact : ArtifactObject
{
    [SerializeField] private float artifactPriceFactor;
    [SerializeField] private float hazardPriceFactor;
    [SerializeField] private int afterRoundGold;
    [SerializeField] private int afterKillGold;
    [SerializeField] private float goldSpawnRateFactor;

    public override int CalculateArtifactPrice(int price)
    {
        return Mathf.RoundToInt(price * artifactPriceFactor);
    }

    public override int CalculateHazardPrice(int price)
    {
        return Mathf.RoundToInt(price * hazardPriceFactor);
    }

    public override void OnFinishRound()
    {
        DataManger.AddGold(afterRoundGold);
    }

    public override void OnKillEnemy(Enemy enemy)
    {
        DataManger.AddGold(afterKillGold);
    }

    public override float CalculateGoldSpawnRate(float rate)
    {
        return rate * goldSpawnRateFactor;
    }
}
