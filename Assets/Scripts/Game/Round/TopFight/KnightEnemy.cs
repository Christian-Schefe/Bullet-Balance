using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class KnightEnemy : Enemy
{
    public override void DoAttackAnimation(Vector3 target)
    {
        var oldPos = transform.position;
        this.TweenPosition().To(target).Duration(0.25f).Ease(Easing.CubicInOut).RepeatWait(0.1f).PingPong(2).RunNew();
    }

    protected override int ComputeHealth(float difficulty)
    {
        return 30 + Mathf.FloorToInt(30 * difficulty);
    }

    public override int CalculateDamage()
    {
        return 2;
    }
}
