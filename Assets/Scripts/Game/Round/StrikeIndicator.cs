using System.Collections;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class StrikeIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outerCircle;
    [SerializeField] private SpriteRenderer innerCircle;

    private float radius;

    public void SetRadius(float radius)
    {
        this.radius = radius;
        outerCircle.transform.localScale = radius * 2 * Vector3.one;
        innerCircle.transform.localScale = Vector2.zero;
    }

    public void RunAnimation(float duration)
    {
        this.TweenScale(innerCircle.transform).From(Vector3.zero).To(radius * 2 * Vector3.one).Duration(duration).Ease(Easing.Linear).OnFinally(() =>
        {
            Destroy(gameObject);
        }).RunNew();
    }
}
