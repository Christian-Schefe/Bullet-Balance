using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSlider : BetterSlider
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private RectTransform rt;
    private Rect Rect => rt != null ? rt.rect : (rt = GetComponent<RectTransform>()).rect;

    protected override Vector2 MinPosition => new(Rect.xMin, Rect.center.y);
    protected override Vector2 MaxPosition => new(Rect.xMax, Rect.center.y);

    protected override void OnProgressChanged(float progress)
    {
        base.OnProgressChanged(progress);
        UpdateText();
    }

    private void UpdateText()
    {
        if (text != null) text.text = Value.ToString();
    }
}
