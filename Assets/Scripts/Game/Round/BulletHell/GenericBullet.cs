using System;
using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public abstract class GenericBullet : Projectile
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

    public override void DealEnemyDamage(TopFight topFight)
    {
        topFight.DealDamageFront(damage);
    }

    public override void DealPlayerDamage(TopFight topFight)
    {
        topFight.AttackPlayer(damage);
    }

    public override bool IsHit(Player player)
    {
        return projectile.IsCollidingWith(player.Collider);
    }

    public override void HandleDestroy(bool playerHit)
    {
        bool instantDestroy = !playerHit;
        projectile.AnimateDestroy(instantDestroy);
    }

    protected abstract void UpdatePosition(float timeLived, out Vector2 dir);

    protected virtual bool PreventDestruction()
    {
        return false;
    }

    public override void Tick(float time, out bool shouldDestroy)
    {
        var timeLived = time - spawnTime;

        UpdatePosition(timeLived, out var dir);

        projectile.Dir = dir;
        projectile.Radius = radius(timeLived);

        shouldDestroy = timeLived > 1 && projectile.IsOutside(arena);
        shouldDestroy |= maxLifetime is float t && timeLived > t;
        if (PreventDestruction()) shouldDestroy = false;
    }
}
