using Tweenables;
using UnityEngine;

public class DOTHazard : IHazard
{
    private readonly ProjectileEntity bulletPrefab;
    private readonly GameObject indicatorPrefab;

    private readonly Curves curves;
    private readonly Settings settings;
    private readonly Variances variances;

    private Arena arena;
    private int level;

    private Timer timer;

    public struct Settings
    {
        public float[] radius;
        public int[] damage;
        public float[] spawnFrequency;
        public float[] maxLifetime;
        public float[] tickDuration;
        public Color bulletColor;

        public Settings(float[] radius, int[] damage, float[] spawnFrequency, float[] maxLifetime, float[] tickDuration, Color bulletColor)
        {
            this.radius = radius;
            this.damage = damage;
            this.spawnFrequency = spawnFrequency;
            this.maxLifetime = maxLifetime;
            this.tickDuration = tickDuration;
            this.bulletColor = bulletColor;
        }
    }

    public struct Curves
    {
        public AnimationCurve radius;

        public Curves(AnimationCurve radius)
        {
            this.radius = radius;
        }
    }

    public struct Variances
    {
        public float radiusVariance;
        public float spawnFrequencyVariance;

        public Variances(float radiusVariance, float spawnFrequencyVariance)
        {
            this.radiusVariance = radiusVariance;
            this.spawnFrequencyVariance = spawnFrequencyVariance;
        }
    }

    private struct SpawnParams
    {
        public float radius;

        public SpawnParams(float radius)
        {
            this.radius = radius;
        }
    }

    public DOTHazard(ProjectileEntity bulletPrefab, GameObject indicatorPrefab, Settings settings, Curves curves, Variances variances)
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
        timer = new RandomTimer(minDuration, maxDuration, 0.5f);

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
            radius = settings.radius[level] + Random.Range(-variances.radiusVariance, variances.radiusVariance)
        };

        var pos = arena.RandomPosition(0);

        arena.TweenDelayedAction(() => Spawn(spawnParams, pos), 0.5f).RunNew();
    }

    private float RadiusCurve(float scalar, float t) => curves.radius.Evaluate(t) * scalar;

    private void Spawn(SpawnParams data, Vector2 pos)
    {
        int levelDamage = settings.damage[level];
        float tickDuration = settings.tickDuration[level];
        float maxLifetime = settings.maxLifetime[level];

        var instance = arena.CreateBulletObject(bulletPrefab);
        var bullet = new GenericArea(instance, pos, levelDamage, tickDuration, maxLifetime, t => RadiusCurve(data.radius, t));
        bullet.projectile.Color = settings.bulletColor;
        arena.AddTickable(bullet);
    }

    private void ShowIndicator(Vector2 position)
    {

    }
}
