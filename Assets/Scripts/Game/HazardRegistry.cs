using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HazardRegistry", menuName = "Game/HazardRegistry")]
public class HazardRegistry : ScriptableObject
{
    [SerializeField] private List<HazardObject> hazards = new();

    private Dictionary<string, HazardObject> hazardsById;
    public IEnumerable<HazardObject> Enumerator => hazards;

    private void GenerateDictionary()
    {
        hazardsById = new Dictionary<string, HazardObject>();
        foreach (var hazard in hazards)
        {
            hazardsById[hazard.hazardId] = hazard;
        }
    }

    public HazardObject Lookup(string id)
    {
        if (hazardsById == null)
        {
            GenerateDictionary();
        }

        return hazardsById[id];
    }
}
