using UnityEngine;

public abstract class Enemy
{
    private int health;
    private int maxHealth;

    protected EnemyEntity entity;
    protected EnemyObject data;

    private int cycleCooldown;

    public Enemy(EnemyEntity entity, EnemyObject data)
    {
        this.entity = entity;
        this.data = data;
        maxHealth = data.CalculateHealth(DataManager.MapData.WorldIndex, DataManager.MapData.CurrentNodeInfo.difficulty);
        health = maxHealth;
        entity.Healthbar.UpdateHealthBar(health, maxHealth);
        entity.SetSpriteAnimation(data.iconSprites);

        cycleCooldown = data.GetCycleTime(EnemyAction.Spawn);
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

    public abstract float DoAttackAnimation(Vector3 target);

    public int CalculateDamage()
    {
        return data.CalculateDamage(DataManager.MapData.WorldIndex, DataManager.MapData.CurrentNodeInfo.difficulty);
    }

    public EnemyAction? DoCycle(bool canMove, bool canMeleeAttack)
    {
        cycleCooldown--;
        if (cycleCooldown > 0)
        {
            return null;
        }

        EnemyAction action = EnemyAction.Idle;

        if (canMove && MovementType == EnemyMovementType.Moving)
        {
            action = EnemyAction.Move;
        }
        else if (AttackType == EnemyAttackType.Ranged || canMeleeAttack)
        {
            action = EnemyAction.Attack;
        }

        cycleCooldown = data.GetCycleTime(action);
        return action;
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