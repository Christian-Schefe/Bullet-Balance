using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tweenables;
using UnityEngine;

public class TopFight : MonoBehaviour
{
    [SerializeField] private WaveGenerator waveGenerator;
    [SerializeField] private DamageNumber damageNumberPrefab;
    [SerializeField] private EnemyEntity enemyEntityPrefab;
    [SerializeField] private Arena arena;
    [SerializeField] private int slotCount;

    [SerializeField] private Transform fromPos, toPos;
    [SerializeField] private Transform playerTarget;

    [SerializeField] private AudioClip playerHitSound, enemyHitSound;

    private Enemy[] enemies;
    private EnemyWave enemyWave;

    private float lastShiftTime;

    private void Start()
    {
        enemies = new Enemy[slotCount];
        var nodeType = DataManger.MapData.CurrentNodeType;

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
        var timeSinceLastShift = time - lastShiftTime;

        if (timeSinceLastShift >= 5)
        {
            lastShiftTime += timeSinceLastShift;
            ShiftEnemies(out var notShiftedEnemies);
            StartCoroutine(DoEnemyAttacks(notShiftedEnemies));
        }

        if (CanSpawnEnemy() && enemyWave.TrySpawn(time, out var data))
        {
            var enemy = Instantiate(enemyEntityPrefab, transform);
            enemy.transform.position = fromPos.position;
            enemies[^1] = data.CreateEnemy(enemy);
            enemy.AnimateSpawn(0.5f);
        }

        hasPlayerWon = enemyWave.IsEmpty && enemies.All(e => e == null);
    }

    private IEnumerator DoEnemyAttacks(List<int> enemies)
    {
        foreach (var i in enemies)
        {
            var enemy = this.enemies[i];
            var attackType = enemy.AttackType;
            if (i != 0 && attackType != EnemyAttackType.Ranged) continue;
            AttackPlayer(enemy.CalculateDamage());
            enemy.DoAttackAnimation(playerTarget.transform.position);

            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool CanSpawnEnemy() => enemies[^1] == null;

    private void UpdateEnemyPosition(int i, bool instant)
    {
        if (enemies[i] == null) return;
        float progress = 1 - i / (float)(enemies.Length - 1);
        var delay = instant ? 0 : 0.1f * i;
        enemies[i].AnimateMove(Vector3.Lerp(fromPos.position, toPos.position, progress), delay, instant);
    }

    private void ShiftEnemies(out List<int> notShiftedEnemies)
    {
        notShiftedEnemies = new List<int>();

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null) continue;

            if (i > 0 && enemies[i - 1] == null)
            {
                enemies[i - 1] = enemies[i];
                enemies[i] = null;
                UpdateEnemyPosition(i - 1, false);
            }
            else notShiftedEnemies.Add(i);
        }
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
}
