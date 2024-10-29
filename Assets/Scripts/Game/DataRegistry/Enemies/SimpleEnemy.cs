using UnityEngine;

[CreateAssetMenu(fileName = "SimpleEnemy", menuName = "Enemies/SimpleEnemy")]
public class SimpleEnemy : EnemyObject
{
    [SerializeField] private Vector2Int healthRange;
    [SerializeField] private int damage;
    [SerializeField] private EnemyMovementType movementType;
    [SerializeField] private EnemyAttackType attackType;
    [SerializeField] private GameObject rangedProjectilePrefab;

    public override EnemyMovementType MovementType => movementType;

    public override EnemyAttackType AttackType => attackType;

    public override int CalculateDamage(int worldIndex, float worldProgress)
    {
        return damage;
    }

    public override int CalculateHealth(int worldIndex, float worldProgress)
    {
        return Mathf.RoundToInt(Mathf.Lerp(healthRange.x, healthRange.y, worldProgress));
    }

    public override Enemy CreateEnemy(EnemyEntity entity)
    {
        if (attackType == EnemyAttackType.Melee)
        {
            return new MeleeEnemy(entity, this);
        }
        else
        {
            return new RangedEnemy(rangedProjectilePrefab, entity, this);
        }
    }
}
