using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tweenables;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TopFight : MonoBehaviour
{
    [SerializeField] private WaveGenerator waveGenerator;
    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private EnemyEntity enemyEntityPrefab;
    [SerializeField] private Arena arena;

    [SerializeField] private int slotCount;
    [SerializeField] private float cycleTime;

    [SerializeField] private Transform fromPos, toPos;
    [SerializeField] private Transform playerTarget;

    [SerializeField] private AudioClip playerHitSound, enemyHitSound;

    private Enemy[] enemies;
    private EnemyWave enemyWave;

    private float lastCycleTime;

    private void Start()
    {
        enemies = new Enemy[slotCount];
        var nodeType = DataManager.MapData.CurrentNodeInfo.nodeType;

        var waveType = nodeType switch
        {
            NodeType.Fight => WaveType.Normal,
            NodeType.HardFight => WaveType.Elite,
            NodeType.Boss => WaveType.Boss,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        enemyWave = waveGenerator.GenerateWave(waveType);

        for (int i = 0; i < slotCount; i++)
        {
            var initialEnemy = enemyWave.GetInitialEnemy(slotCount - 1 - i);
            if (initialEnemy != null)
            {
                var enemy = Instantiate(enemyEntityPrefab, transform);
                enemies[i] = initialEnemy.CreateEnemy(enemy);
                UpdateEnemyPosition(i, true);
                enemy.AnimateSpawn(0);
            }
        }
    }

    public void Tick(float time, out bool hasPlayerWon)
    {
        var timeSinceLastCycle = time - lastCycleTime;

        if (timeSinceLastCycle >= cycleTime)
        {
            lastCycleTime += timeSinceLastCycle;
            StartCoroutine(DoCycle(time));
        }

        hasPlayerWon = enemyWave.IsEmpty && enemies.All(e => e == null);
    }

    private bool CanSpawnEnemy() => enemies[^1] == null;

    private void UpdateEnemyPosition(int i, bool instant)
    {
        if (enemies[i] == null) return;
        float progress = 1 - i / (float)(enemies.Length - 1);
        enemies[i].AnimateMove(Vector3.Lerp(fromPos.position, toPos.position, progress), 0, instant);
    }

    public void DealDamageFront(int damage)
    {
        int i = 0;
        while (i < enemies.Length && enemies[i] == null)
        {
            i++;
        }
        if (i < enemies.Length)
        {
            AttackEnemy(i, damage);
        }
    }

    public void DealPierceDamage(int damage, int? pierce)
    {
        int pierced = 0;
        int i = 0;

        while (i < enemies.Length && (pierce is not int p || pierced < p))
        {
            if (enemies[i] != null)
            {
                AttackEnemy(i, damage);
                pierced++;
            }
            i++;
        }
    }

    public void DealSplashDamage(int damage, int diameter)
    {
        int i = 0;
        while (i < enemies.Length && enemies[i] == null)
        {
            i++;
        }
        for (int j = 0; j < diameter; j++)
        {
            var pos = i + j;
            if (pos >= enemies.Length) return;
            if (enemies[pos] != null)
            {
                AttackEnemy(pos, damage);
            }
        }
    }

    public void AttackPlayer(int damage)
    {
        arena.DealDamage(damage);
        var damageNumber = Instantiate(damageNumberPrefab, transform);
        damageNumber.DisplayDamage(playerTarget, damage, true);

        SfxSystem.PlaySfx(playerHitSound);
    }

    private void AttackEnemy(int index, int damage)
    {
        var enemy = enemies[index];
        var isDead = enemy.DealDamage(damage);
        if (isDead)
        {
            enemy.Die();
            enemies[index] = null;

            Signals.Get<ArenaOnKillEnemy>().Dispatch(enemy);
        }

        SfxSystem.PlaySfx(enemyHitSound);
    }

    private IEnumerator DoCycle(float time)
    {
        for (int i = 0; i < slotCount; i++)
        {
            var enemy = enemies[i];
            if (enemy == null) continue;

            bool canMove = CanMove(i, out var targetIndex);
            bool canMeleeAttack = i == 0;
            var action = enemy.DoCycle(canMove, canMeleeAttack);

            if (action == EnemyAction.Move)
            {
                DoMove(i, targetIndex);
            }
            else if (action == EnemyAction.Attack)
            {
                var animDuration = enemy.DoAttackAnimation(playerTarget.transform.position);
                this.RunDelayed(() =>
                {
                    AttackPlayer(enemy.CalculateDamage());
                }, animDuration);
            }
            else if (action != null && action != EnemyAction.Idle)
            {
                throw new System.ArgumentException();
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (CanSpawnEnemy() && enemyWave.TrySpawn(time, out var data))
        {
            var enemy = Instantiate(enemyEntityPrefab, transform);
            enemy.transform.position = fromPos.position;
            enemies[^1] = data.CreateEnemy(enemy);
            enemy.AnimateSpawn(0.0f);
        }
    }

    private void DoMove(int index, int targetIndex)
    {
        if (enemies[index] == null || enemies[targetIndex] != null) throw new System.ArgumentException();

        var enemy = enemies[index];
        enemies[targetIndex] = enemy;
        enemies[index] = null;

        float progress = 1 - targetIndex / (float)(enemies.Length - 1);
        var pos = Vector3.Lerp(fromPos.position, toPos.position, progress);
        enemy.AnimateMove(pos, 0, false);
    }

    private bool CanMove(int index, out int targetIndex)
    {
        int canSkip = 1;
        targetIndex = index - 1;
        while (targetIndex >= 0 && canSkip >= 0)
        {
            if (enemies[targetIndex] == null) return true;
            targetIndex--;
            canSkip--;
        }
        return false;
    }
}

public enum EnemyAction
{
    Idle,
    Spawn,
    Attack,
    Move
}