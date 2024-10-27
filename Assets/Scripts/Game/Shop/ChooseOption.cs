using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseOption : MonoBehaviour
{
    [SerializeField] private Image selectorImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private ClickableIcon clickableIcon;
    public ClickableIcon ClickableIcon => clickableIcon;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    public bool Selected
    {
        set => selectorImage.enabled = value;
    }

    public int Index { get; set; }
}
