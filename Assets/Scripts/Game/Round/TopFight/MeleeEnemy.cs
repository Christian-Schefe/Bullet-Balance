using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : Enemy
{
    public MeleeEnemy(EnemyEntity entity, EnemyObject data) : base(entity, data)
    {
    }

    public override void DoAttackAnimation(Vector3 target)
    {
        entity.AnimateMeleeAttack(target);
    }
}
