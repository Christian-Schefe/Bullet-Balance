using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBullet : GenericBullet
{
    private Func<float, float> speed;
    private Func<float, Vector2> path;

    private float pathProgress;

    public PathBullet(ProjectileObject projectile, int damage, float? maxLifetime, Func<float, float> speed, Func<float, float> radius, Func<float, Vector2> path) : base(projectile, path(0), damage, maxLifetime, radius)
    {
        this.speed = speed;
        this.path = path;

        pathProgress = 0;
    }

    protected override void UpdatePosition(float timeLived, out Vector2 dir)
    {
        pathProgress = Mathf.Clamp(pathProgress + speed(timeLived) * Time.deltaTime, 0, 1);
        var newPos = path(pathProgress);
        var diff = newPos - projectile.Pos;
        dir = diff.normalized;
        projectile.Pos = newPos;
    }
}
