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
        DataManger.IncreaseMaxHealth(addedMaxHealth);
    }

    public override void OnFinishRound()
    {
        DataManger.HealPlayer(afterRoundHeal);
    }

    public override void OnKillEnemy(Enemy enemy)
    {
        DataManger.HealPlayer(afterKillHeal);
    }

    public override int CalculateHealPrice(int price)
    {
        return Mathf.RoundToInt(price * healPriceFactor);
    }

    public override int CalculateHealAmount(int amount)
    {
        return Mathf.RoundToInt(amount * healAmountFactor);
    }
}
