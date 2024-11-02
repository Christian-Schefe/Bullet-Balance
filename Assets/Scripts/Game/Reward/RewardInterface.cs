using System.Collections.Generic;
using UnityEngine;

public class RewardInterface : MonoBehaviour
{
    [SerializeField] private HazardRegistry hazardRegistry;
    [SerializeField] private ClickableIcon hazardIconPrefab;

    [SerializeField] private PriceButton healButton;
    [SerializeField] private BetterButton finishButton;
    [SerializeField] private BetterButton openUpgradeScreenButton;
    [SerializeField] private BetterButton closeUpgradeScreenButton;
    [SerializeField] private RectTransform upgradeScreen;
    [SerializeField] private RectTransform upgradeGrid;
    [SerializeField] private RectTransform upgradeSelector;
    [SerializeField] private PriceButton upgradeButton;

    private RunManager runManager;
    private InventoryData inventoryData;

    private List<ClickableIcon> hazardIcons = new();
    private int? selectedIcon;

    private bool hasHealed;
    private bool hasUpgraded;

    private void Start()
    {
        runManager = Globals<RunManager>.Instance;
        inventoryData = DataManager.InventoryData;

        healButton.onClick.AddListener(OnHeal);
        DataManager.StatsData.healPriceStore.AddSceneListener(OnHealPriceChanged);
        DataManager.StatsData.healAmountStore.AddSceneListener(OnHealAmountChanged);
        healButton.AvailableCount = DataManager.PlayerData.Health < DataManager.PlayerData.MaxHealth ? 1 : 0;

        finishButton.onClick.AddListener(OnFinish);
        openUpgradeScreenButton.onClick.AddListener(OpenUpgradeScreen);
        closeUpgradeScreenButton.onClick.AddListener(CloseUpgradeScreen);

        upgradeButton.onClick.AddListener(UpgradeSelection);
        upgradeButton.AvailableCount = 1;

        upgradeScreen.gameObject.SetActive(false);
        DeselectUpgradeIcon();
    }

    private void OnHealPriceChanged(bool isPresent, float price)
    {
        healButton.SetPrice(Mathf.RoundToInt(price));
    }

    private void OnHealAmountChanged(bool isPresent, float amount)
    {
        healButton.Text = $"Heal By {Mathf.RoundToInt(amount)}";
    }

    private void OpenUpgradeScreen()
    {
        upgradeScreen.gameObject.SetActive(true);
        SpawnInventory(inventoryData.HazardLevels);
    }

    private void CloseUpgradeScreen()
    {
        upgradeScreen.gameObject.SetActive(false);
        DeselectUpgradeIcon();
        DestroyInventory();
    }

    private void UpgradeSelection()
    {
        if (selectedIcon is not int index) return;
        if (hasUpgraded) return;
        var level = inventoryData.HazardLevels[index].Item2;
        var price = GetUpgradePrice(level);
        if (price is not int priceValue) return;
        if (!DataManager.TrySpendGold(priceValue)) return;

        hasUpgraded = true;
        upgradeButton.AvailableCount -= 1;

        inventoryData.SetLevel(index, level + 1);
        hazardIcons[index].SetLevel(level + 1);
        DeselectUpgradeIcon();
    }

    private void SpawnInventory(List<(string, int)> inventory)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            var (type, level) = inventory[i];
            var index = i;
            var instance = Instantiate(hazardIconPrefab, upgradeGrid);
            var hazard = hazardRegistry.Lookup(type);
            instance.Sprite = hazard.iconSprite;
            instance.OnClick.AddListener(() => SelectUpgradeIcon(index));
            instance.Level = level;
            instance.GetTooltip().SetData(hazard.GetTooltipData(level));
            hazardIcons.Add(instance);
        }
    }

    private void DestroyInventory()
    {
        foreach (var icon in hazardIcons)
        {
            Destroy(icon.gameObject);
        }
        hazardIcons.Clear();
    }

    private void DeselectUpgradeIcon()
    {
        selectedIcon = null;
        upgradeSelector.gameObject.SetActive(false);
        upgradeButton.SetPrice(null);
        upgradeButton.Interactable = false;
    }

    private int? GetUpgradePrice(int level)
    {
        var prices = DataManager.StatsData.HazardUpgradePrices;
        if (level >= prices.Length) return null;
        return Mathf.RoundToInt(prices[level]);
    }

    private void SelectUpgradeIcon(int index)
    {
        selectedIcon = index;
        upgradeSelector.gameObject.SetActive(true);
        upgradeSelector.transform.position = hazardIcons[index].transform.position;
        var level = inventoryData.HazardLevels[index].Item2;
        int? price = GetUpgradePrice(level);
        upgradeButton.SetPrice(price);
    }

    private void OnHeal()
    {
        if (hasHealed) return;
        if (DataManager.PlayerData.Health >= DataManager.PlayerData.MaxHealth) return;
        if (!DataManager.TrySpendGold(Mathf.RoundToInt(DataManager.StatsData.HealPrice))) return;

        hasHealed = true;
        healButton.AvailableCount -= 1;
        DataManager.HealPlayer(Mathf.RoundToInt(DataManager.StatsData.HealAmount));
    }

    private void OnFinish()
    {
        runManager.LoadScene(SceneType.Map);
    }
}
