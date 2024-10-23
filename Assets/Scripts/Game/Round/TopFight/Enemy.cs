using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private int health;
    private int maxHealth;

    [SerializeField] private EntityHealthbar healthbar;
    [SerializeField] private DamageNumber damageNumberPrefab;

    private TweenRunner runner;
    private EnemyType type;
    public EnemyType Type => type;

    public void AnimateMove(Vector3 to, float delay)
    {
        this.TweenPosition().To(to).Delay(delay).Duration(0.5f).Ease(Easing.CubicInOut).RunQueued(ref runner);
    }

    public bool DealDamage(int damage)
    {
        health = Mathf.Max(0, health - damage);
        healthbar.UpdateHealthBar(health, maxHealth);

        var damageNumber = Instantiate(damageNumberPrefab, transform);
        damageNumber.DisplayDamage(transform, damage, false);

        return health == 0;
    }

    public void SetHealth(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
        healthbar.UpdateHealthBar(health, maxHealth);
    }

    protected abstract int ComputeHealth(float difficulty);

    public void Die()
    {
        this.TweenScale().To(Vector3.zero).Duration(0.5f).Ease(Easing.CubicIn).OnComplete(() => Destroy(gameObject)).RunNew();
    }

    public void Spawn(EnemyType type, float difficulty, float delay)
    {
        this.type = type;
        transform.localScale = Vector3.zero;
        this.TweenScale().From(Vector3.zero).To(Vector3.one).Delay(delay).Duration(0.5f).Ease(Easing.CubicOut).RunNew();
        var health = ComputeHealth(difficulty);
        SetHealth(health, health);
    }

    public abstract void DoAttackAnimation(Vector3 target);

    public abstract int CalculateDamage();
}
