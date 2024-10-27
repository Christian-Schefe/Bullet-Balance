using System;
using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    public void SetPath(Func<float, Vector2> path)
    {
        var points = new List<Vector3>();
        for (var i = 0; i < 100; i++)
        {
            var t = i / 100f;
            var point = path(t);
            points.Add(point);
        }

        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }

    public void RunAnimation(float duration)
    {
        this.TweenAny<float>().Use(t =>
        {
            line.widthMultiplier = t;
        }).From(0).To(1).Duration(0.1f).Ease(Easing.Linear).RunNew();

        this.TweenAny<float>().Use(t =>
        {
            var gradient = line.colorGradient;
            var newGradient = new Gradient();
            newGradient.SetKeys(
                gradient.colorKeys,
                new GradientAlphaKey[] {
                    new(t * 0.15f, 0),
                    new(0, 0.85f)
                }
            );
            line.colorGradient = newGradient;

        }).From(1).To(0).Duration(duration).Ease(Easing.Linear).OnFinally(() =>
        {
            Destroy(gameObject);
        }).RunNew();
    }
}
