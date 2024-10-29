using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private Node nodePrefab;
    [SerializeField] private Connection connectionPrefab;

    [SerializeField] private NodeRegistry nodeRegistry;

    [SerializeField] private float randomOffset;
    [SerializeField] private Vector2 nodeSpacing;
    [SerializeField] private Vector2 nodeLayerSpacing;
    [SerializeField] private float connectionSpacing;

    private Dictionary<Node, List<Node>> connectionLookup;

    private Dictionary<Vector2Int, Node> nodeObjects;
    private List<Connection> connectionObjects;

    private SeededRandom random;

    public Rect BoundingBox { get; private set; } = Rect.zero;
    public System.Action<Node> onNodeClicked;

    public ICollection<Node> Nodes => nodeObjects.Values;

    public void Build(SeededRandom random, List<List<ExtendedNodeType>> layers, List<List<List<int>>> connections)
    {
        connectionLookup = new();

        nodeObjects = new();
        connectionObjects = new();
        this.random = random;

        for (int y = 0; y < layers.Count; y++)
        {
            var layer = layers[y];
            SpawnLayer(y, layer);
        }

        for (int y = 0; y < layers.Count; y++)
        {
            var layer = connections[y];
            SpawnConnections(y, layer);
        }

        CalculateBoundingBox();
    }

    public void Destroy()
    {
        foreach (var (_, node) in nodeObjects)
        {
            Destroy(node.gameObject);
        }

        foreach (var connection in connectionObjects)
        {
            Destroy(connection.gameObject);
        }

        nodeObjects.Clear();
        connectionObjects.Clear();

        connectionLookup.Clear();
        BoundingBox = Rect.zero;
    }

    private void CalculateBoundingBox()
    {
        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);

        foreach (var (_, node) in nodeObjects)
        {
            var pos = node.transform.position;

            min = Vector2.Min(min, pos);
            max = Vector2.Max(max, pos);
        }

        BoundingBox = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }

    private void SpawnLayer(int layer, List<ExtendedNodeType> nodeLayer)
    {
        for (int x = 0; x < nodeLayer.Count; x++)
        {
            var pos = new Vector2Int(x, layer);
            SpawnNode(nodeLayer.Count, pos, nodeLayer[x]);
        }
    }

    private void SpawnNode(int layerSize, Vector2Int pos, ExtendedNodeType nodeType)
    {
        var remappedX = pos.x - (layerSize - 1) * 0.5f;

        Vector2 localPos = remappedX * 4f / (layerSize + 1) * nodeSpacing + pos.y * nodeLayerSpacing;

        var sprite = nodeRegistry[nodeType.appearanceType].icon;
        var color = nodeRegistry[nodeType.appearanceType].color;

        var randomOffsetVec = random.InsideUnitCircle() * randomOffset;

        var node = Instantiate(nodePrefab, transform);
        node.transform.localPosition = localPos + randomOffsetVec;
        node.ConfigureNode(nodeType.FunctionalType, pos);
        node.SetSprite(sprite, color);
        node.onClick += node => onNodeClicked?.Invoke(node);

        nodeObjects.Add(pos, node);
    }

    private void SpawnConnections(int layer, List<List<int>> connectionLayer)
    {
        for (int fromX = 0; fromX < connectionLayer.Count; fromX++)
        {
            var fromPos = new Vector2Int(fromX, layer);
            var fromNode = nodeObjects[fromPos];
            var connections = new List<Node>();

            foreach (var toX in connectionLayer[fromX])
            {
                var toPos = new Vector2Int(toX, layer + 1);
                var toNode = nodeObjects[toPos];

                var connection = SpawnConnection(fromNode, toNode);
                connections.Add(toNode);
                connectionObjects.Add(connection);
            }

            connectionLookup.Add(fromNode, connections);
        }
    }

    private Connection SpawnConnection(Node from, Node to)
    {
        var connection = Instantiate(connectionPrefab, transform);
        connection.Set(from, to, connectionSpacing);
        return connection;
    }

    public Node GetNode(Vector2Int position)
    {
        return nodeObjects[position];
    }

    public SceneType GetSceneType(Vector2Int position)
    {
        return GetSceneType(GetNode(position).NodeType);
    }

    public SceneType GetSceneType(NodeType nodeType)
    {
        return nodeRegistry[nodeType].scene;
    }

    public List<Node> GetConnectedNodes(Vector2Int position)
    {
        return connectionLookup[GetNode(position)];
    }
}
