using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public class ArcherEnemy : Enemy
{
    [SerializeField] private GameObject arrowPrefab;

    public override void DoAttackAnimation(Vector3 target)
    {
        var arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        var oldPos = arrow.transform.position;
        new Tween<Vector3>(this).Use(delegate (Vector3 p)
        {
            arrow.transform.position = p;
        }).From(oldPos).To(target).Duration(0.25f).Ease(Easing.CubicIn).OnFinally(() => Destroy(arrow)).RunNew();
    }

    protected override int ComputeHealth(float difficulty)
    {
        return 20 + Mathf.FloorToInt(20 * difficulty);
    }

    public override int CalculateDamage()
    {
        return 1;
    }
}
