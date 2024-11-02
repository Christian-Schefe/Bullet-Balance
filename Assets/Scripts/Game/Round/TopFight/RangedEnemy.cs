using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    private GameObject projectilePrefab;

    public RangedEnemy(GameObject projectilePrefab, EnemyEntity entity, EnemyObject data) : base(entity, data)
    {
        this.projectilePrefab = projectilePrefab;
    }

    public override float DoAttackAnimation(Vector3 target)
    {
        var instance = Object.Instantiate(projectilePrefab, entity.transform.position, Quaternion.identity);
        entity.AnimateRangedAttack(instance.transform, target, () => { Object.Destroy(instance); }, 0.05f, out var duration);
        return duration;
    }
}
