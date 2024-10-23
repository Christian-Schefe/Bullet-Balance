using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuyOption : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private PriceButton selectButton;
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

    public int? Price
    {
        get => selectButton.Price;
        set => selectButton.Price = value;
    }

    public int? AvailableCount
    {
        get => selectButton.AvailableCount;
        set => selectButton.AvailableCount = value;
    }
}
