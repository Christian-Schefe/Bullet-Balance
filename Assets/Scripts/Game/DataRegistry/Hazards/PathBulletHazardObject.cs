using UnityEngine;

[CreateAssetMenu(fileName = "PathBulletHazardObject", menuName = "Hazards/PathBulletHazardObject")]
public class PathBulletHazardObject : HazardObject
{
    [SerializeField] private ProjectileEntity bulletPrefab;
    [SerializeField] private PathIndicator indicatorPrefab;

    [SerializeField] private float[] speed, radius;
    [SerializeField] private AnimationCurve speedCurve, radiusCurve;
    [SerializeField] private int[] damage;
    [SerializeField] private float[] spawnFrequency;
    [SerializeField] private float[] maxLifetime;
    [SerializeField] private float spawnFrequencyVariance, speedVariance, radiusVariance;
    [SerializeField] private Color bulletColor;

    public override IHazard CreateHazard()
    {
        var settings = new PathBulletHazard.Settings(speed, radius, damage, spawnFrequency, maxLifetime, bulletColor);
        var curves = new PathBulletHazard.Curves(speedCurve, radiusCurve);
        var variances = new PathBulletHazard.Variances(radiusVariance, speedVariance, spawnFrequencyVariance);
        return new PathBulletHazard(bulletPrefab, indicatorPrefab, settings, curves, variances);
    }

    public override TooltipData GetTooltipData(int level)
    {
        var baseData = base.GetTooltipData(level);
        baseData.damage = damage[level];
        return baseData;
    }
}
