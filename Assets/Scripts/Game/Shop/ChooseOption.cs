using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChooseOption : MonoBehaviour
{
    [SerializeField] private Image image, selectorImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private BetterButton selectButton;
    public UnityEvent OnClick => selectButton.onClick;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    public Sprite Sprite
    {
        get => image.sprite;
        set => image.sprite = value;
    }

    public bool Selected
    {
        set => selectorImage.enabled = value;
    }

    public int Index { get; set; }
}
