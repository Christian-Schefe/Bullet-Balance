using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ClickableHazardIcon : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private BetterButton button;
    [SerializeField] private TextMeshProUGUI levelText;

    private readonly static string[] romanNumerals = new string[]
    {
        "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"
    };

    public void Set(Sprite icon, UnityAction onClick)
    {
        image.sprite = icon;
        button.onClick.AddListener(onClick);
    }

    public void SetLevel(int level)
    {
        levelText.text = romanNumerals[level];
    }
}
