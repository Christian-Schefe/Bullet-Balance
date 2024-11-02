using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DOTHazardObject", menuName = "Hazards/DOTHazardObject")]
public class DOTHazardObject : HazardObject
{
    [SerializeField] private ProjectileEntity bulletPrefab;
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField] private float[] radius;
    [SerializeField] private AnimationCurve radiusCurve;
    [SerializeField] private int[] damage;
    [SerializeField] private float[] spawnFrequency;
    [SerializeField] private float[] maxLifetime;
    [SerializeField] private float[] tickDuration;
    [SerializeField] private float spawnFrequencyVariance, radiusVariance;
    [SerializeField] private Color bulletColor;

    public override IHazard CreateHazard()
    {
        var settings = new DOTHazard.Settings(radius, damage, spawnFrequency, maxLifetime, tickDuration, bulletColor);
        var curves = new DOTHazard.Curves(radiusCurve);
        var variances = new DOTHazard.Variances(radiusVariance, spawnFrequencyVariance);
        return new DOTHazard(bulletPrefab, indicatorPrefab, settings, curves, variances);
    }

    public override TooltipData GetTooltipData(int level)
    {
        var baseData = base.GetTooltipData(level);
        baseData.damage = damage[level];
        return baseData;
    }
}

