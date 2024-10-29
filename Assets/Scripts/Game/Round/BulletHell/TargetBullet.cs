using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBullet : GenericBullet
{
    private Func<float, float> speed;
    private Func<float, float> follow;

    public TargetBullet(ProjectileEntity projectile, Vector2 pos, int damage, float? maxLifetime, Func<float, float> speed, Func<float, float> radius, Func<float, float> follow) : base(projectile, pos, damage, maxLifetime, radius)
    {
        projectile.Dir = (arena.Player.Position - projectile.Pos).normalized;

        this.speed = speed;
        this.follow = follow;
    }

    protected override void UpdatePosition(float timeLived, out Vector2 dir)
    {
        var playerDir = (arena.Player.Position - projectile.Pos).normalized;
        dir = Vector3.Slerp(projectile.Dir, playerDir, follow(timeLived) * Time.deltaTime).normalized;

        var velocity = speed(timeLived) * dir;
        projectile.Pos += velocity * Time.deltaTime;
    }
}
