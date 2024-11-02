using System;
using UnityEngine;

public abstract class GenericBullet : ITickable
{
    public ProjectileEntity projectile;

    protected Arena arena;

    protected int damage;
    protected float? maxLifetime;

    protected Func<float, float> radius;

    protected float spawnTime;

    public GenericBullet(ProjectileEntity projectile, Vector2 pos, int damage, float? maxLifetime, Func<float, float> radius)
    {
        this.projectile = projectile;
        arena = Globals<Arena>.Instance;

        this.damage = damage;
        this.maxLifetime = maxLifetime;
        this.radius = radius;

        spawnTime = arena.GameTime;

        projectile.Pos = pos;
        projectile.Radius = radius(0);
    }

    protected abstract void UpdatePosition(float timeLived, out Vector2 dir);

    protected virtual bool PreventDestruction()
    {
        return false;
    }

    public void Tick(float time)
    {
        var timeLived = time - spawnTime;

        UpdatePosition(timeLived, out var dir);

        projectile.Dir = dir;
        projectile.Radius = radius(timeLived);

        var shouldDestroy = timeLived > 1 && projectile.IsOutside(arena);
        shouldDestroy |= maxLifetime is float t && timeLived > t;
        if (PreventDestruction()) shouldDestroy = false;

        if (shouldDestroy)
        {
            arena.TopFight.DealDamageFront(damage);
            projectile.AnimateDestroy(0.0f, true);
        }
        else if (projectile.IsCollidingWith(arena.Player.Collider))
        {
            shouldDestroy = true;
            arena.TopFight.AttackPlayer(damage);
            projectile.AnimateDestroy(0.1f, false);
        }

        if (shouldDestroy)
        {
            arena.ScheduleRemoveTickable(this);
        }
    }
}
