using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public abstract class Enemy
{
    private int health;
    private int maxHealth;

    protected EnemyEntity entity;
    protected EnemyObject data;

    public Enemy(EnemyEntity entity, EnemyObject data)
    {
        this.entity = entity;
        this.data = data;
        maxHealth = data.CalculateHealth(DataManger.MapData.WorldIndex, DataManger.MapData.CurrentNodeInfo.difficulty);
        health = maxHealth;
        entity.Healthbar.UpdateHealthBar(health, maxHealth);
        entity.SetSpriteAnimation(data.iconSprites);
    }

    public EnemyAttackType AttackType => data.AttackType;
    public EnemyMovementType MovementType => data.MovementType;

    public void AnimateMove(Vector3 to, float delay, bool instant)
    {
        entity.AnimateMove(to, delay, instant);
    }

    public bool DealDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        entity.Healthbar.UpdateHealthBar(health, maxHealth);
        entity.AnimateTakeDamage(damage);

        return health == 0;
    }

    public void Die()
    {
        entity.AnimateDestroy();
    }

    public void Spawn(float delay)
    {
        entity.AnimateSpawn(delay);
    }

    public abstract void DoAttackAnimation(Vector3 target);

    public int CalculateDamage()
    {
        return data.CalculateDamage(DataManger.MapData.WorldIndex, DataManger.MapData.CurrentNodeInfo.difficulty);
    }
}

public enum EnemyAttackType
{
    Melee,
    Ranged
}

public enum EnemyMovementType
{
    Stationary,
    Moving
}