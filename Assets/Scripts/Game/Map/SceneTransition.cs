using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    private TweenRunner runner;
    private bool isTransitioning = false;

    [SerializeField] private float duration;

    private void Awake()
    {
        sprite.transform.localScale = new Vector3(22, 22, 1);
        SetProgress(0);
        isTransitioning = false;

        TransitionAwake();
    }

    public bool TransitionLoadScene(SceneReference scene)
    {
        if (isTransitioning)
        {
            print("Already Transitioning...");
            return false;
        }
        isTransitioning = true;
        runner.Cancel();
        sprite.enabled = true;
        new Tween<float>(this).From(1).To(0).Use(t => SetProgress(t)).Duration(duration).Ease(Easing.CubicIn).OnFinally(() =>
        {
            isTransitioning = false;
            SceneSystem.LoadScene(scene);
        }).RunImmediate(ref runner);
        return true;
    }

    private void SetProgress(float progress)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Progress", Mathf.Clamp01(progress));
        sprite.SetPropertyBlock(propertyBlock);
    }

    public void TransitionAwake()
    {
        sprite.enabled = true;
        SetProgress(0);
        new Tween<float>(this).Delay(0.25f).From(0).To(1).Use(t => SetProgress(t)).Duration(duration).Ease(Easing.CubicOut).OnFinally(() =>
        {
            sprite.enabled = false;
        }).RunImmediate(ref runner);
    }
}
