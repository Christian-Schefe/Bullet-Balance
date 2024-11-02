using System;
using UnityEngine;

public class GenericArea : ITickable
{
    public ProjectileEntity projectile;

    protected Arena arena;

    protected int damage;
    protected float maxLifetime;
    protected float tickDuration;

    protected Func<float, float> radius;

    protected float spawnTime;

    private float lastTickTime;

    public GenericArea(ProjectileEntity projectile, Vector2 pos, int damage, float tickDuration, float maxLifetime, Func<float, float> radius)
    {
        this.projectile = projectile;
        arena = Globals<Arena>.Instance;

        this.damage = damage;
        this.maxLifetime = maxLifetime;
        this.radius = radius;
        this.tickDuration = tickDuration;

        spawnTime = arena.GameTime;
        lastTickTime = spawnTime;

        projectile.Pos = pos;
        projectile.Radius = radius(0);
    }

    protected virtual void UpdatePosition(float timeLived) { }

    public void Tick(float time)
    {
        var timeLived = time - spawnTime;

        UpdatePosition(timeLived);

        projectile.Radius = radius(timeLived);

        var timeSinceTick = arena.GameTime - lastTickTime;
        if (timeSinceTick >= tickDuration)
        {
            lastTickTime += tickDuration;
            if (projectile.IsCollidingWith(arena.Player.Collider))
            {
                arena.TopFight.AttackPlayer(damage);
            }
            else
            {
                arena.TopFight.DealDamageFront(damage);
            }
        }

        var shouldDestroy = timeLived > maxLifetime;

        if (shouldDestroy)
        {
            projectile.AnimateDestroy(0.5f, false);
            arena.ScheduleRemoveTickable(this);
        }
    }
}