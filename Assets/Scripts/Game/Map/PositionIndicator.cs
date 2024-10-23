using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class PositionIndicator : MonoBehaviour
{
    public void AnimateMove(Node target)
    {
        this.TweenPosition().To(target.transform.position).Duration(0.5f).Ease(Easing.CubicInOut).RunNew();
    }
}
