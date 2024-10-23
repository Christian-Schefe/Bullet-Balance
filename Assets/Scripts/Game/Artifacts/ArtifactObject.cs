using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtifactObject : ScriptableObject
{
    public string artifactName;
    public string artifactId;
    public Sprite iconSprite;

    public virtual void OnAquire()
    {

    }

    public virtual void OnFinishRound()
    {

    }

    public virtual void OnKillEnemy(Enemy enemy)
    {

    }

    public virtual int ModifyPlayerDamage(int damage)
    {
        return damage;
    }

    public virtual int ModifyEnemyDamage(int damage)
    {
        return damage;
    }

    public virtual int CalculateArtifactPrice(int price)
    {
        return price;
    }

    public virtual int CalculateHazardPrice(int price)
    {
        return price;
    }

    public virtual int CalculateHealPrice(int price)
    {
        return price;
    }

    public virtual int CalculateHealAmount(int amount)
    {
        return amount;
    }

    public virtual float CalculateGoldSpawnRate(float rate)
    {
        return rate;
    }
}