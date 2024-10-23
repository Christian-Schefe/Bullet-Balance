using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweenables.Core;
using Tweenables;
using UnityEngine;

public class SfxButton : BetterButton
{
    [SerializeField] private AudioClip hoverClip, pressClip;

    protected override void ToState(State state)
    {
        var oldState = CurrentState;
        base.ToState(state);

        if (oldState == state) return;

        if (oldState != State.Pressed && state == State.Hovered)
        {
            SfxSystem.PlaySfx(hoverClip);
        }
        else if (state == State.Pressed)
        {
            SfxSystem.PlaySfx(pressClip);
        }
    }
}
