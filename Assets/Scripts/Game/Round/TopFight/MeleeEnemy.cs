using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public MeleeEnemy(EnemyEntity entity, EnemyObject data) : base(entity, data)
    {
    }

    public override float DoAttackAnimation(Vector3 target)
    {
        var duration = 0.3f;
        entity.AnimateMeleeAttack(target, duration);
        return duration;
    }
}
