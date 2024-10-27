using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BuyOption : MonoBehaviour
{
    [SerializeField] private ClickableIcon icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private PriceButton selectButton;
    public UnityEvent OnClick => selectButton.onClick;

    public string Name
    {
        get => nameText.text;
        set => nameText.text = value;
    }

    public ClickableIcon Icon => icon;

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
