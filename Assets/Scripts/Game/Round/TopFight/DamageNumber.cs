using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweenables;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Color playerDamageColor;
    [SerializeField] private Color enemyDamageColor;

    public void DisplayDamage(Transform target, int damage, bool isPlayerDamage)
    {
        transform.position = target.position;

        text.text = damage.ToString();
        text.color = isPlayerDamage ? playerDamageColor : enemyDamageColor;

        var start = transform.position;
        var end = start + new Vector3(0.5f, 0.2f, 0f);
        var control = (start + end) / 2 + Vector3.up * 0.4f;

        var bezier = GetQuadBezier(start, control, end);

        var positionTween = this.TweenAny<float>().From(0).To(1).Duration(0.75f).Ease(Easing.CubicOut);
        positionTween.Use(t => transform.position = bezier(t)).OnFinally(() =>
        {
            Destroy(gameObject);
        }).RunNew();

        var alphaTween = this.TweenAny<float>().From(1).To(0).Duration(0.75f).Ease(Easing.Linear);
        alphaTween.Use(t => text.alpha = t).RunNew();
    }

    private Func<float, Vector3> GetQuadBezier(Vector3 start, Vector3 control, Vector3 end)
    {
        return t => (1 - t) * (1 - t) * start + 2 * (1 - t) * t * control + t * t * end;
    }
}
