using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweenables;
using Tweenables.Core;
using UnityEngine;
using UnityEngine.UI;

public class TweenButton : SfxButton
{
    [SerializeField] private Color interactableColor, disabledColor;
    [SerializeField] private float interactableScale, hoveredScale, pressedScale, disabledScale;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image image;
    [TextArea][SerializeField] private string text;
    [SerializeField] private float tweenDuration;

    private TweenRunner colorTween, scaleTween;

    public string Text
    {
        get => text;
        set
        {
            text = value;
            label.text = value;
        }
    }

    protected override void Awake()
    {
        ResetState();
    }

    private void ResetState()
    {
        if (label != null) label.text = text;
        if (image != null) image.color = Interactable ? interactableColor : disabledColor;
        transform.localScale = Vector3.one * (Interactable ? interactableScale : disabledScale);
    }

    protected override void OnEnable()
    {
        ResetState();
    }

    private Color GetTargetColor(State state)
    {
        return state switch
        {
            State.Interactable => interactableColor,
            State.Hovered => DarkenColor(interactableColor, 0.9f),
            State.Pressed => DarkenColor(interactableColor, 0.8f),
            State.Disabled => disabledColor,
            _ => interactableColor
        };
    }

    private Vector3 GetTargetScale(State state)
    {
        return Vector3.one * state switch
        {
            State.Interactable => interactableScale,
            State.Hovered => hoveredScale,
            State.Pressed => pressedScale,
            State.Disabled => disabledScale,
            _ => interactableScale
        };
    }

    protected override void ToState(State state)
    {
        if (image != null)
        {
            var oldColor = image.color;
            var newColor = GetTargetColor(state);
            var colorTween = new Tween<Color>().Owner(this).Use(color => image.color = color);
            colorTween.From(oldColor).To(newColor).Duration(tweenDuration).Ease(Easing.QuadInOut);

            if (Application.isPlaying && isActiveAndEnabled) colorTween.RunImmediate(ref this.colorTween);
            else image.color = newColor;
        }

        var oldScale = transform.localScale;
        var newScale = GetTargetScale(state);
        var scaleTween = this.TweenScale().From(oldScale).To(newScale).Duration(tweenDuration).Ease(Easing.QuadInOut);

        if (Application.isPlaying && isActiveAndEnabled) scaleTween.RunImmediate(ref this.scaleTween);
        else transform.localScale = newScale;

        base.ToState(state);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (Application.isPlaying) return;
        base.OnValidate();
        ResetState();
    }
#endif

    private Color DarkenColor(Color color, float factor)
    {
        Color.RGBToHSV(color, out var h, out var s, out var v);
        return Color.HSVToRGB(h, s, v * factor);
    }
}
