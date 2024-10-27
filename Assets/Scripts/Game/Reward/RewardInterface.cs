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

    [SerializeField] private int healAmount;
    [SerializeField] private int healPrice;

    private RunManager runManager;
    private InventoryData inventoryData;

    private int[] upgradePrices = new int[] { 10, 20 };

    private List<ClickableIcon> hazardIcons = new();
    private int? selectedIcon;

    private bool hasHealed;
    private bool hasUpgraded;

    private void Start()
    {
        runManager = Globals<RunManager>.Instance;
        inventoryData = DataManger.InventoryData;

        healButton.onClick.AddListener(OnHeal);
        healButton.SetPrice(GetHealPrice());
        healButton.Text = $"Heal By {GetHealAmount()}";
        healButton.AvailableCount = DataManger.PlayerData.Health < DataManger.PlayerData.MaxHealth ? 1 : 0;

        finishButton.onClick.AddListener(OnFinish);
        openUpgradeScreenButton.onClick.AddListener(OpenUpgradeScreen);
        closeUpgradeScreenButton.onClick.AddListener(CloseUpgradeScreen);

        upgradeButton.onClick.AddListener(UpgradeSelection);
        upgradeButton.AvailableCount = 1;

        upgradeScreen.gameObject.SetActive(false);
        DeselectUpgradeIcon();
    }

    public int GetHealPrice()
    {
        return Globals<ArtifactApplier>.Instance.CalculateHealPrice(healPrice);
    }

    public int GetHealAmount()
    {
        return Globals<ArtifactApplier>.Instance.CalculateHealAmount(healAmount);
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
        if (level > upgradePrices.Length) return;
        var price = upgradePrices[level];
        if (!DataManger.TrySpendGold(price)) return;

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

    private void SelectUpgradeIcon(int index)
    {
        selectedIcon = index;
        upgradeSelector.gameObject.SetActive(true);
        upgradeSelector.transform.position = hazardIcons[index].transform.position;
        var level = inventoryData.HazardLevels[index].Item2;
        int? price = level < upgradePrices.Length ? upgradePrices[level] : null;
        upgradeButton.SetPrice(price);
    }

    private void OnHeal()
    {
        if (hasHealed) return;
        if (DataManger.PlayerData.Health >= DataManger.PlayerData.MaxHealth) return;
        if (!DataManger.TrySpendGold(GetHealPrice())) return;

        hasHealed = true;
        healButton.AvailableCount -= 1;
        DataManger.HealPlayer(GetHealAmount());
    }

    private void OnFinish()
    {
        runManager.LoadScene(SceneType.Map);
    }
}
