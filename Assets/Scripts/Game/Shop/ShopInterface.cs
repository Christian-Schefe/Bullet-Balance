using System.Collections.Generic;
using UnityEngine;

public class ShopInterface : MonoBehaviour
{
    [SerializeField] private HazardRegistry hazardRegistry;
    [SerializeField] private ArtifactRegistry artifactRegistry;

    [SerializeField] private Transform artifactsContainer, hazardsContainer;
    [SerializeField] private BuyOption shopOptionPrefab;

    [SerializeField] private int artifactCount, hazardCount;

    private List<string> artifactTypes = new();
    private readonly List<(string, int)?> hazardTypes = new();

    private List<BuyOption> artifactOptions = new(), hazardOptions = new();

    public void Awake()
    {
        var rng = new SeededRandom(DataManager.MapData.CurrentNodeInfo.sceneSeed);

        artifactTypes = RandomArtifactTypes(rng, artifactCount, artifactRegistry);
        var hazardIds = RandomHazardTypes(rng, hazardCount, hazardRegistry);

        for (var i = 0; i < artifactTypes.Count; i++)
        {
            var type = artifactTypes[i];
            var artifact = artifactRegistry.Lookup(type);
            BuyOption option = Instantiate(shopOptionPrefab, artifactsContainer);

            var index = i;
            option.Icon.Sprite = artifact.iconSprite;
            option.Name = artifact.artifactName;
            option.AvailableCount = 1;
            option.OnClick.AddListener(() => BuyArtifact(option, index));
            option.Icon.Level = null;
            option.Icon.GetTooltip().SetData(artifact.GetTooltipData());
            artifactOptions.Add(option);
        }

        for (var i = 0; i < hazardIds.Count; i++)
        {
            var type = hazardIds[i];
            var hazard = hazardRegistry.Lookup(type);
            BuyOption option = Instantiate(shopOptionPrefab, hazardsContainer);

            var level = 0;
            hazardTypes.Add((type, level));

            var index = i;
            option.Icon.Sprite = hazard.iconSprite;
            option.Name = hazard.hazardName;
            option.AvailableCount = 1;
            option.OnClick.AddListener(() => BuyHazard(option, index));
            option.Icon.Level = null;
            option.Icon.GetTooltip().SetData(hazard.GetTooltipData(level));
            hazardOptions.Add(option);
        }

        DataManager.StatsData.hazardPriceStore.AddSceneListener(OnHazardPriceChanged);
        DataManager.StatsData.artifactPriceStore.AddSceneListener(OnArtifactPriceChanged);
    }

    private void OnHazardPriceChanged(bool isPresent, float price)
    {
        var intPrice = Mathf.RoundToInt(price);
        foreach (var option in hazardOptions)
        {
            option.Price = intPrice;
        }
    }

    private void OnArtifactPriceChanged(bool isPresent, float price)
    {
        var intPrice = Mathf.RoundToInt(price);
        foreach (var option in artifactOptions)
        {
            option.Price = intPrice;
        }
    }

    private void BuyArtifact(BuyOption option, int index)
    {
        var type = artifactTypes[index];
        if (type is null) return;
        if (option.Price is not int p || !DataManager.TrySpendGold(p)) return;

        artifactTypes[index] = null;
        option.AvailableCount -= 1;

        DataManager.InventoryData.AddArtifact(type);
    }

    private void BuyHazard(BuyOption option, int index)
    {
        var data = hazardTypes[index];
        if (data is not (string type, int level)) return;
        if (option.Price is not int p || !DataManager.TrySpendGold(p)) return;

        hazardTypes[index] = null;
        option.AvailableCount -= 1;

        DataManager.InventoryData.AddHazard(type, level);
    }

    public static int HazardPrice()
    {
        return Mathf.RoundToInt(DataManager.StatsData.HazardPrice);
    }

    public static int ArtifactPrice()
    {
        return Mathf.RoundToInt(DataManager.StatsData.ArtifactPrice);
    }

    public static List<string> RandomArtifactTypes(SeededRandom rng, int amount, ArtifactRegistry registry)
    {
        var owned = DataManager.InventoryData.Artifacts;

        var notOwned = new List<string>();

        foreach (var artifact in registry.Objects)
        {
            if (!owned.Contains(artifact.artifactId))
            {
                notOwned.Add(artifact.artifactId);
            }
        }

        var chosen = new List<string>();

        for (var i = 0; i < amount; i++)
        {
            if (notOwned.Count == 0) break;

            var index = rng.IntRange(0, notOwned.Count);
            var type = notOwned[index];
            notOwned.Remove(type);
            chosen.Add(type);
        }

        return chosen;
    }

    public static List<string> RandomHazardTypes(SeededRandom rng, int amount, HazardRegistry hazards)
    {
        var allSet = new List<string>();

        foreach (var hazard in hazards.Objects)
        {
            allSet.Add(hazard.hazardId);
        }

        var chosen = new List<string>();

        for (var i = 0; i < amount; i++)
        {
            if (allSet.Count == 0) break;

            var index = rng.IntRange(0, allSet.Count);
            var type = allSet[index];
            allSet.Remove(type);
            chosen.Add(type);
        }

        return chosen;
    }
}
