using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthArtifact", menuName = "Artifacts/HealthArtifact")]
public class HealthArtifact : ArtifactObject
{
    [SerializeField] private int addedMaxHealth;
    [SerializeField] private int afterRoundHeal;
    [SerializeField] private int afterKillHeal;
    [SerializeField] private float healPriceFactor;
    [SerializeField] private float healAmountFactor;

    public override void OnAquire()
    {
        DataManager.IncreaseMaxHealth(addedMaxHealth);
        DataManager.StatsData.HealPrice *= healPriceFactor;
        DataManager.StatsData.HealAmount *= healAmountFactor;
    }

    public override void OnFinishRound()
    {
        DataManager.HealPlayer(afterRoundHeal);
    }

    public override void OnKillEnemy(Enemy enemy)
    {
        DataManager.HealPlayer(afterKillHeal);
    }
}
