using System.Collections;
using System.Collections.Generic;
using Tweenables;
using Tweenables.Core;
using UnityEngine;

public class GoldEntity : MonoBehaviour, ITickable
{
    [SerializeField] private AudioClip collectGoldSound;

    public Arena arena;

    private TweenRunner tweenRunner;

    public void Tick(float time)
    {
        var dist = Vector2.Distance(transform.position, arena.Player.Position);
        if (dist <= DataManager.StatsData.GoldPickupDistance)
        {
            Collect();
        }
    }

    public void Spawn()
    {
        this.TweenScale().From(Vector3.zero).To(Vector3.one).Duration(0.15f).Ease(Easing.CubicOut).RunImmediate(ref tweenRunner);
    }

    private void Collect()
    {
        arena.ScheduleRemoveTickable(this);
        DataManager.AddGold(1);
        this.TweenScale().To(Vector3.zero).From(Vector3.one).Duration(0.15f).Ease(Easing.CubicIn).OnFinally(() => Destroy(gameObject)).RunImmediate(ref tweenRunner);
        this.TweenAny<float>().From(0).To(1).Duration(0.5f).Use(t => transform.position = Vector2.Lerp(transform.position, arena.Player.Position, t)).RunNew();
        SfxSystem.PlaySfx(collectGoldSound);
    }
}
