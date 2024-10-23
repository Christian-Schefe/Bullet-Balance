using System;
using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class GenericBullet : Projectile
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider2d;

    private Arena arena;

    private Vector2 dir;
    private int damage;
    private float? maxLifetime;

    private Func<float, float> radius;
    private Func<float, float> speed;
    private Func<float, float> follow;

    private float timeLived;
    private float spawnTime;

    public void Spawn(Arena arena, Vector2 pos, Vector2 dir, int damage, float? maxLifetime, Func<float, float> speed, Func<float, float> radius, Func<float, float> follow)
    {
        this.arena = arena;

        transform.position = pos;
        this.dir = dir.normalized;
        this.damage = damage;
        this.maxLifetime = maxLifetime;

        this.speed = speed;
        this.radius = radius;
        this.follow = follow;

        timeLived = 0;
        spawnTime = arena.GameTime;

        transform.localScale = 2 * radius(0) * Vector3.one;
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
        return player.Collider.IsTouching(collider2d);
    }

    public override void PlayerHitDestroy()
    {
        this.TweenScale().To(Vector3.zero).Duration(0.1f).OnComplete(() => Destroy(gameObject)).RunNew();
    }

    public override void Tick(float time, out bool shouldDestroy)
    {
        timeLived = time - spawnTime;

        var playerDir = (arena.Player.Position - (Vector2)transform.position).normalized;
        dir = Vector3.Slerp(dir, playerDir, follow(timeLived) * Time.deltaTime).normalized;

        var velocity = speed(timeLived) * dir;
        transform.position += (Vector3)velocity * Time.deltaTime;

        shouldDestroy = timeLived > 1 && arena.IsFullyOutside(transform.position, radius(timeLived));
        shouldDestroy |= maxLifetime is float t && timeLived > t;

        var angle = -Vector2.SignedAngle(dir, Vector2.up);
        transform.rotation = Quaternion.Euler(new(0, 0, angle));

        var curScale = 2 * radius(timeLived) * Vector3.one;

        transform.localScale = curScale;
    }
}
