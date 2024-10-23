using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletHazard", menuName = "Hazards/BulletHazard")]
public class BulletHazardObject : HazardObject
{
    [SerializeField] private GenericBullet bulletPrefab;
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField] private float[] speed, radius, follow;
    [SerializeField] private AnimationCurve speedCurve, radiusCurve, followCurve;
    [SerializeField] private int[] damage;
    [SerializeField] private float[] spawnFrequency;
    [SerializeField] private float[] maxLifetime;
    [SerializeField] private float spawnFrequencyVariance, speedVariance, radiusVariance;

    public override IHazard CreateHazard()
    {
        var settings = new GenericBulletHazard.Settings()
        {
            speed = speed,
            radius = radius,
            follow = follow,
            damage = damage,
            spawnFrequency = spawnFrequency,
            maxLifetime = maxLifetime,
        };
        var curves = new GenericBulletHazard.Curves()
        {
            speed = speedCurve,
            radius = radiusCurve,
            follow = followCurve,
        };
        var variances = new GenericBulletHazard.Variances()
        {
            radiusVariance = radiusVariance,
            speedVariance = speedVariance,
            spawnFrequencyVariance = spawnFrequencyVariance,
        };
        return new GenericBulletHazard(bulletPrefab, indicatorPrefab, settings, curves, variances);
    }
}
