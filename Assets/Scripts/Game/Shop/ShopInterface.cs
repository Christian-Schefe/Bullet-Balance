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

    public void Awake()
    {
        artifactTypes = RandomArtifactTypes(artifactCount, artifactRegistry);
        var hazardIds = RandomHazardTypes(hazardCount, hazardRegistry);

        for (var i = 0; i < artifactTypes.Count; i++)
        {
            var type = artifactTypes[i];
            var artifact = artifactRegistry.Lookup(type);
            BuyOption option = Instantiate(shopOptionPrefab, artifactsContainer);

            var index = i;
            var price = ArtifactPrice();
            option.Icon.Sprite = artifact.iconSprite;
            option.Name = artifact.name;
            option.Price = price;
            option.AvailableCount = 1;
            option.OnClick.AddListener(() => BuyArtifact(option, index, price));
            option.Icon.Level = null;
            option.Icon.GetTooltip().SetData(artifact.GetTooltipData());
        }

        for (var i = 0; i < hazardIds.Count; i++)
        {
            var type = hazardIds[i];
            var hazard = hazardRegistry.Lookup(type);
            BuyOption option = Instantiate(shopOptionPrefab, hazardsContainer);

            var level = 0;
            hazardTypes.Add((type, level));

            var index = i;
            var price = HazardPrice(level);
            option.Icon.Sprite = hazard.iconSprite;
            option.Name = hazard.name;
            option.Price = price;
            option.AvailableCount = 1;
            option.OnClick.AddListener(() => BuyHazard(option, index, price));
            option.Icon.Level = null;
            option.Icon.GetTooltip().SetData(hazard.GetTooltipData(level));
        }
    }

    private void BuyArtifact(BuyOption option, int index, int price)
    {
        var type = artifactTypes[index];
        if (type is null) return;
        if (!DataManger.TrySpendGold(price)) return;

        artifactTypes[index] = null;
        option.AvailableCount -= 1;

        print("Bought " + type);
        DataManger.InventoryData.AddArtifact(type);
    }

    private void BuyHazard(BuyOption option, int index, int price)
    {
        var data = hazardTypes[index];
        if (data is not (string type, int level)) return;
        if (!DataManger.TrySpendGold(price)) return;

        hazardTypes[index] = null;
        option.AvailableCount -= 1;

        DataManger.InventoryData.AddHazard(type, level);
    }

    public static int HazardPrice(int level)
    {
        var basePrice = 15 + 10 * level;
        return Globals<ArtifactApplier>.Instance.CalculateHazardPrice(basePrice);
    }

    public static int ArtifactPrice()
    {
        var basePrice = 15;
        return Globals<ArtifactApplier>.Instance.CalculateArtifactPrice(basePrice);
    }

    public static List<string> RandomArtifactTypes(int amount, ArtifactRegistry registry)
    {
        var owned = DataManger.InventoryData.Artifacts;

        var notOwned = new List<string>();

        foreach (var artifact in registry.Enumerator)
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

            var index = Random.Range(0, notOwned.Count);
            var type = notOwned[index];
            notOwned.Remove(type);
            chosen.Add(type);
        }

        return chosen;
    }

    public static List<string> RandomHazardTypes(int amount, HazardRegistry hazards)
    {
        var allSet = new List<string>();

        foreach (var hazard in hazards.Enumerator)
        {
            allSet.Add(hazard.hazardId);
        }

        var chosen = new List<string>();

        for (var i = 0; i < amount; i++)
        {
            if (allSet.Count == 0) break;

            var index = Random.Range(0, allSet.Count);
            var type = allSet[index];
            allSet.Remove(type);
            chosen.Add(type);
        }

        return chosen;
    }
}
