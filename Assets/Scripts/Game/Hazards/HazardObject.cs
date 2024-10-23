using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HazardObject : ScriptableObject
{
    public string hazardName;
    public string hazardId;
    public Sprite iconSprite;

    public abstract IHazard CreateHazard();
}
