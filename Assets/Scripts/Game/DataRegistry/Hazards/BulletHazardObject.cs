using UnityEngine;

[CreateAssetMenu(fileName = "BulletHazard", menuName = "Hazards/BulletHazard")]
public class BulletHazardObject : HazardObject
{
    [SerializeField] private ProjectileEntity bulletPrefab;
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField] private float[] speed, radius, follow;
    [SerializeField] private AnimationCurve speedCurve, radiusCurve, followCurve;
    [SerializeField] private int[] damage;
    [SerializeField] private float[] spawnFrequency;
    [SerializeField] private float[] maxLifetime;
    [SerializeField] private float spawnFrequencyVariance, speedVariance, radiusVariance;
    [SerializeField] private Color bulletColor;

    public override IHazard CreateHazard()
    {
        var settings = new TargetBulletHazard.Settings(speed, radius, follow, damage, spawnFrequency, maxLifetime, bulletColor);
        var curves = new TargetBulletHazard.Curves(speedCurve, radiusCurve, followCurve);
        var variances = new TargetBulletHazard.Variances(speedVariance, radiusVariance, spawnFrequencyVariance);
        return new TargetBulletHazard(bulletPrefab, indicatorPrefab, settings, curves, variances);
    }

    public override TooltipData GetTooltipData(int level)
    {
        var baseData = base.GetTooltipData(level);
        baseData.damage = damage[level];
        return baseData;
    }
}
