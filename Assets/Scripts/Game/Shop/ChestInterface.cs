using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestInterface : MonoBehaviour
{
    [SerializeField] private ArtifactRegistry artifactRegistry;

    [SerializeField] private Transform artifactsContainer;
    [SerializeField] private ChooseOption choicePrefab;
    [SerializeField] private PriceButton chooseArtifactButton;

    [SerializeField] private int artifactCount;
    [SerializeField] private bool isFree;

    private List<string> artifactTypes = new();
    private readonly List<ChooseOption> chooseOptions = new();

    private ChooseOption selectedArtifact;

    public void Awake()
    {
        artifactTypes = ShopInterface.RandomArtifactTypes(artifactCount, artifactRegistry);

        for (var i = 0; i < artifactTypes.Count; i++)
        {
            var type = artifactTypes[i];
            var artifact = artifactRegistry.Lookup(type);
            ChooseOption option = Instantiate(choicePrefab, artifactsContainer);
            chooseOptions.Add(option);

            option.Sprite = artifact.iconSprite;
            option.Name = artifact.name;
            option.Index = i;
            option.OnClick.AddListener(() => SelectArtifact(option));
            option.Selected = false;

            if (i == 0) SelectArtifact(option);
        }

        chooseArtifactButton.onClick.AddListener(ChooseArtifact);

        chooseArtifactButton.Price = null;

        chooseArtifactButton.AvailableCount = Mathf.Min(artifactTypes.Count, 1);
        chooseArtifactButton.ActiveWithoutPrice = isFree;
    }

    private void ChooseArtifact()
    {
        if (selectedArtifact == null) return;
        if (!isFree && !DataManger.TrySpendGold(ShopInterface.ArtifactPrice())) return;

        var type = artifactTypes[selectedArtifact.Index];
        DataManger.InventoryData.AddArtifact(type);
        chooseArtifactButton.AvailableCount--;

        foreach (var option in chooseOptions)
        {
            option.Selected = false;
            option.OnClick.RemoveAllListeners();
        }
    }

    private void SelectArtifact(ChooseOption option)
    {
        if (selectedArtifact != null) selectedArtifact.Selected = false;
        selectedArtifact = option;
        option.Selected = true;

        chooseArtifactButton.Price = isFree ? null : ShopInterface.ArtifactPrice();
    }
}
