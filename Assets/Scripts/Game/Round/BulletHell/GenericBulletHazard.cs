using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class GenericBulletHazard : IHazard
{
    private readonly GenericBullet bulletPrefab;
    private readonly GameObject indicatorPrefab;

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
        public float[] follow;
        public int[] damage;
        public float[] spawnFrequency;
        public float[] maxLifetime;
    }

    public struct Curves
    {
        public AnimationCurve speed;
        public AnimationCurve radius;
        public AnimationCurve follow;
    }

    public struct Variances
    {
        public float speedVariance;
        public float radiusVariance;
        public float spawnFrequencyVariance;
    }

    private struct SpawnParams
    {
        public float speed;
        public float radius;
    }

    public GenericBulletHazard(GenericBullet bulletPrefab, GameObject indicatorPrefab, Settings settings, Curves curves, Variances variances)
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

        var posArr = arena.RandomEdgePosition(0.25f, RadiusCurve(spawnParams.radius, 0));
        var indicatorPos = posArr[0];
        var pos = posArr[1];
        ShowIndicator(indicatorPos);

        arena.TweenDelayedAction(() => Spawn(spawnParams, pos), 0.5f).RunNew();
    }

    private float SpeedCurve(float scalar, float t) => curves.speed.Evaluate(t) * scalar;
    private float RadiusCurve(float scalar, float t) => curves.radius.Evaluate(t) * scalar;
    private float FollowCurve(float t) => curves.follow.Evaluate(t) * settings.follow[level];

    private void Spawn(SpawnParams data, Vector2 pos)
    {
        var instance = arena.CreateBullet(bulletPrefab);
        var dirToPlayer = (arena.Player.Position - pos).normalized;

        int levelDamage = settings.damage[level];

        float? maxLifetime = settings.maxLifetime[level];
        if (maxLifetime <= 0) maxLifetime = null;

        instance.Spawn(arena, pos, dirToPlayer, levelDamage, maxLifetime, t => SpeedCurve(data.speed, t), t => RadiusCurve(data.radius, t), FollowCurve);
    }

    private void ShowIndicator(Vector2 position)
    {
        var instance = Object.Instantiate(indicatorPrefab, position, Quaternion.identity);
        var runner = arena.TweenScale(instance.transform).From(Vector3.zero).To(Vector3.one).Duration(0.25f).Ease(Easing.CubicOut).RunNew();
        arena.TweenScale(instance.transform).Delay(0.25f).From(Vector3.one).To(Vector3.zero).Duration(0.25f).Ease(Easing.CubicIn).OnFinally(() =>
        {
            Object.Destroy(instance);
        }).RunQueued(ref runner);
    }
}
