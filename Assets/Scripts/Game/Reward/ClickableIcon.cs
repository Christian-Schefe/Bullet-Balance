using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tweenables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClickableIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private BetterButton button;
    [SerializeField] private TextMeshProUGUI levelText;

    private readonly static string[] romanNumerals = new string[]
    {
        "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"
    };

    private int? level;
    private Tooltip tooltip;

    public int? Level
    {
        get => level;
        set => SetLevel(value);
    }

    public Sprite Sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    public Tooltip GetTooltip()
    {
        if (tooltip == null) tooltip = Globals<TooltipManager>.Instance.CreateTooltip();
        return tooltip;
    }

    public UnityEvent OnClick => button.onClick;

    private void Awake()
    {
        button.AddHoverListener(OnHover);
        SetLevel(level);
    }

    private void OnHover(bool hover)
    {
        var tt = GetTooltip();
        if (hover) tt.ShowAt(transform.position);
        else tt.SetOpen(false);
    }

    public void SetLevel(int? level)
    {
        this.level = level;
        levelText.text = level is int l ? romanNumerals[l] : "";
    }
}
