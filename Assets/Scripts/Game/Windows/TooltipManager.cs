using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] private Tooltip tooltipPrefab;
    [SerializeField] private RectTransform tooltipParent;

    public Tooltip CreateTooltip()
    {
        var tooltip = Instantiate(tooltipPrefab, tooltipParent);
        tooltip.SetOpen(false);
        return tooltip;
    }
}
