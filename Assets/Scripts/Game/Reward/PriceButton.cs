using TMPro;
using UnityEngine;

public class PriceButton : TweenButton
{
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private GameObject priceLabelContainer;

    private int? price = null;
    private int? goldValue = null;
    private int? availableCount = null;
    private bool activeWithoutPrice = false;

    public int? AvailableCount
    {
        get => availableCount;
        set => SetAvailableCount(value);
    }

    public int? Price
    {
        get => price;
        set => SetPrice(value);
    }

    public bool ActiveWithoutPrice
    {
        get => activeWithoutPrice;
        set
        {
            activeWithoutPrice = value;
            UpdateState();
        }
    }

    protected override void Awake()
    {
        DataManager.PlayerData.goldStore.AddSceneListener(OnUpdateGold);
    }

    public void SetAvailableCount(int? availableCount)
    {
        this.availableCount = availableCount;
        UpdateState();
    }

    public void SetPrice(int? price)
    {
        this.price = price;

        priceLabelContainer.SetActive(price.HasValue);
        priceLabel.text = price.HasValue ? price.ToString() : "";
        UpdateState();
    }

    private void OnUpdateGold(bool isPresent, int gold)
    {
        goldValue = gold;
        UpdateState();
    }

    private void UpdateState()
    {
        bool soldOut = availableCount.HasValue && availableCount == 0;
        bool canAfford = price is int p && goldValue is int g && g >= p;
        bool activeAndFree = activeWithoutPrice && price is null;
        SetInteractable(!soldOut && (activeAndFree || canAfford));
    }
}
