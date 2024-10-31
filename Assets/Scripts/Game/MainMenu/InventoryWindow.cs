using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : MonoBehaviour, IWindow
{
    [SerializeField] private HazardRegistry hazardRegistry;
    [SerializeField] private ArtifactRegistry artifactRegistry;

    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform hazardContainer, artifactContainer;

    [SerializeField] private ClickableIcon iconPrefab;

    private readonly Dictionary<(string, int), ClickableIcon> hazardIcons = new();
    private readonly Dictionary<string, ClickableIcon> artifactIcons = new();

    private void Awake()
    {
        DataManger.InventoryData.hazardLevelsStore.AddSceneListener(OnHazardsChanged);
        DataManger.InventoryData.artifactsStore.AddSceneListener(OnArtifactChanged);
    }

    public void SetOpen(bool open)
    {
        content.gameObject.SetActive(open);
        Signals.Get<InventoryIsOpen>().Dispatch(open);
    }

    private void OnHazardsChanged(bool isPresent, List<(string, int)> hazards)
    {
        if (!isPresent || hazards == null) return;

        var keysSet = new HashSet<(string, int)>(hazardIcons.Keys);

        foreach (var key in hazards)
        {
            keysSet.Remove(key);
            if (hazardIcons.ContainsKey(key)) continue;

            var obj = hazardRegistry.Lookup(key.Item1);
            var icon = Instantiate(iconPrefab, hazardContainer);
            icon.Sprite = obj.iconSprite;
            icon.Level = key.Item2;
            var tooltipData = obj.GetTooltipData(key.Item2);
            icon.GetTooltip().SetData(tooltipData);

            hazardIcons.Add(key, icon);
        }

        foreach (var key in keysSet)
        {
            Destroy(hazardIcons[key]);
            hazardIcons.Remove(key);
        }
    }

    private void OnArtifactChanged(bool isPresent, HashSet<string> artifacts)
    {
        if (!isPresent || artifacts == null) return;

        var keysSet = new HashSet<string>(artifactIcons.Keys);

        foreach (var key in artifacts)
        {
            keysSet.Remove(key);
            if (artifactIcons.ContainsKey(key)) continue;

            var obj = artifactRegistry.Lookup(key);
            var icon = Instantiate(iconPrefab, artifactContainer);
            icon.Sprite = obj.iconSprite;
            icon.Level = null;
            icon.GetTooltip().SetData(obj.GetTooltipData());

            artifactIcons.Add(key, icon);
        }

        foreach (var key in keysSet)
        {
            Destroy(artifactIcons[key]);
            artifactIcons.Remove(key);
        }
    }

    public void Open(Action closeCallback)
    {
        SetOpen(true);
    }

    public void Close()
    {
        SetOpen(false);
    }
}
