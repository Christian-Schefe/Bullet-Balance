using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ArtifactRegistry), menuName = "Game/ArtifactRegistry")]
public class ArtifactRegistry : ScriptableObject
{
    [SerializeField] private List<ArtifactObject> artifacts = new();

    private Dictionary<string, ArtifactObject> artifactsById;
    public IEnumerable<ArtifactObject> Enumerator => artifacts;

    private void GenerateDictionary()
    {
        artifactsById = new Dictionary<string, ArtifactObject>();
        foreach (var artifact in artifacts)
        {
            artifactsById[artifact.artifactId] = artifact;
        }
    }

    public ArtifactObject Lookup(string id)
    {
        if (artifactsById == null)
        {
            GenerateDictionary();
        }

        return artifactsById[id];
    }
}
