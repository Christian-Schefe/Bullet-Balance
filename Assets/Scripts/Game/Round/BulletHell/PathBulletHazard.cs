using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class PathBulletHazard : IHazard
{
    private readonly ProjectileObject bulletPrefab;
    private readonly PathIndicator indicatorPrefab;

    private readonly Curves curves;
    private readonly Settings settings;
    private readonly Variances variances;

    private Arena arena;
    private int level;

    private Timer timer;

    public struct Settings
    {
        public float[] speed;
        public float[] radius;
        public int[] damage;
        public float[] spawnFrequency;
        public float[] maxLifetime;
        public Color bulletColor;

        public Settings(float[] speed, float[] radius, int[] damage, float[] spawnFrequency, float[] maxLifetime, Color bulletColor)
        {
            this.speed = speed;
            this.radius = radius;
            this.damage = damage;
            this.spawnFrequency = spawnFrequency;
            this.maxLifetime = maxLifetime;
            this.bulletColor = bulletColor;
        }
    }

    public struct Curves
    {
        public AnimationCurve speed;
        public AnimationCurve radius;

        public Curves(AnimationCurve speed, AnimationCurve radius)
        {
            this.speed = speed;
            this.radius = radius;
        }
    }

    public struct Variances
    {
        public float speedVariance;
        public float radiusVariance;
        public float spawnFrequencyVariance;

        public Variances(float speedVariance, float radiusVariance, float spawnFrequencyVariance)
        {
            this.speedVariance = speedVariance;
            this.radiusVariance = radiusVariance;
            this.spawnFrequencyVariance = spawnFrequencyVariance;
        }
    }

    private struct SpawnParams
    {
        public float speed;
        public float radius;
    }

    public PathBulletHazard(ProjectileObject bulletPrefab, PathIndicator indicatorPrefab, Settings settings, Curves curves, Variances variances)
    {
        this.bulletPrefab = bulletPrefab;
        this.indicatorPrefab = indicatorPrefab;
        this.settings = settings;
        this.curves = curves;
        this.variances = variances;
    }

    public void Init(Arena arena, int level)
    {
        this.arena = arena;
        this.level = level;

        var minDuration = settings.spawnFrequency[level] - variances.spawnFrequencyVariance;
        var maxDuration = settings.spawnFrequency[level] + variances.spawnFrequencyVariance;
        timer = new RandomTimer(minDuration, maxDuration);

        timer.action += TimerAction;
    }

    public void Tick(float time)
    {
        timer.Tick(time);
    }

    private void TimerAction(float time)
    {
        var spawnParams = new SpawnParams()
        {
            radius = settings.radius[level] + Random.Range(-variances.radiusVariance, variances.radiusVariance),
            speed = settings.speed[level] + Random.Range(-variances.speedVariance, variances.speedVariance)
        };

        var posArr = arena.RandomEdgePosition(0.25f, 0.5f);
        var indicatorPos = posArr[0];
        var startPos = posArr[1];
        var endPos = arena.RandomEdgePosition(0.5f)[0];
        var path = QuadSplinePath(startPos, arena.Player.Position, endPos);
        ShowIndicator(indicatorPos, path);

        arena.TweenDelayedAction(() => Spawn(spawnParams, path), 0.5f).RunNew();
    }

    private float SpeedCurve(float scalar, float t) => curves.speed.Evaluate(t) * scalar;
    private float RadiusCurve(float scalar, float t) => curves.radius.Evaluate(t) * scalar;
    private System.Func<float, Vector2> QuadSplinePath(Vector2 start, Vector2 middle, Vector2 end)
    {
        return t =>
        {
            var a = Vector2.Lerp(start, middle, t);
            var b = Vector2.Lerp(middle, end, t);
            return Vector2.Lerp(a, b, t);
        };
    }

    private void Spawn(SpawnParams data, System.Func<float, Vector2> path)
    {
        int levelDamage = settings.damage[level];

        float? maxLifetime = settings.maxLifetime[level];
        if (maxLifetime <= 0) maxLifetime = null;

        var instance = arena.CreateBulletObject(bulletPrefab);
        var bullet = new PathBullet(instance, levelDamage, maxLifetime, t => SpeedCurve(data.speed, t), t => RadiusCurve(data.radius, t), path);
        bullet.projectile.Color = settings.bulletColor;
        arena.AddBullet(bullet);
    }

    private void ShowIndicator(Vector2 position, System.Func<float, Vector2> path)
    {
        var instance = Object.Instantiate(indicatorPrefab, position, Quaternion.identity);
        instance.SetPath(path);
        instance.RunAnimation(2.5f);
    }
}
