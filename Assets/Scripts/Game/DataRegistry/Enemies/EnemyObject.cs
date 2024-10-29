using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyObject : ScriptableObject, IRegistryObject
{
    public string enemyName;
    public string enemyId;
    public Sprite[] iconSprites;

    public abstract EnemyMovementType MovementType { get; }
    public abstract EnemyAttackType AttackType { get; }

    public string Id => enemyId;

    public abstract int CalculateDamage(int worldIndex, float worldProgress);
    public abstract int CalculateHealth(int worldIndex, float worldProgress);

    public abstract Enemy CreateEnemy(EnemyEntity entity);
}
