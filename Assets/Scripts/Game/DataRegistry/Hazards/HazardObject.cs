using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardObject : ScriptableObject, IRegistryObject
{
    public string hazardName;
    public string[] tooltipDescriptions;
    public string hazardId;
    public Sprite iconSprite;

    public virtual TooltipData GetTooltipData(int level) => new(hazardName, tooltipDescriptions[level], 0);

    public abstract IHazard CreateHazard();

    public string Id => hazardId;
}
