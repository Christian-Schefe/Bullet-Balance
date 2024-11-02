using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtifactObject : ScriptableObject, IRegistryObject
{
    public string artifactName;
    public string tooltipDescription;
    public string artifactId;
    public Sprite iconSprite;

    public string Id => artifactId;

    public TooltipData GetTooltipData() => new(artifactName, tooltipDescription, null);

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
}