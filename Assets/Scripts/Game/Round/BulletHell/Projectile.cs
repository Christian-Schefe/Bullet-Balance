using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile
{
    public abstract void Tick(float time, out bool shouldDestroy);
    public abstract bool IsHit(Player player);
    public abstract void DealPlayerDamage(TopFight topFight);
    public abstract void DealEnemyDamage(TopFight topFight);
    public abstract void HandleDestroy(bool playerHit);
}
