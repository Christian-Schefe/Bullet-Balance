using System.Collections.Generic;
using Tweenables;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private World world;

    [SerializeField] private PositionIndicator positionIndicatorPrefab;

    private (Vector2, Vector2)? dragStart;
    private List<Node> selectableNodes = new();
    private PositionIndicator positionIndicator;
    private SeededRandom random;

    private Node CurrentNode => world.GetNode(PlayerPosition);
    private WorldLayout WorldData => DataManager.MapData.WorldList.worlds[DataManager.MapData.WorldIndex];
    private MapData MapData => DataManager.MapData;
    private Vector2Int PlayerPosition => MapData.PlayerPosition;

    private void Awake()
    {
        random = new SeededRandom(DataManager.RunData.Seed);

        if (MapData.WorldList.worlds == null)
        {
            print("Generating new map");
            GenerateNewMap();
        }

        var lastLayer = WorldData.nodeTypes.Count - 1;
        var bossPosition = new Vector2Int(WorldData.nodeTypes[lastLayer].Count - 1, lastLayer);
        if (MapData.PlayerPosition == bossPosition)
        {
            if (MapData.WorldList.HasWorld(MapData.WorldIndex + 1))
            {
                MapData.WorldIndex++;
                MapData.PlayerPosition = Vector2Int.zero;
            }
            else
            {
                this.TweenDelayedAction(() =>
                {
                    Globals<RunManager>.Instance.Win();
                }, 0.5f).RunNew();
            }
        }
    }

    private void GenerateNewMap()
    {
        var worlds = new List<WorldLayout>();

        for (int i = 0; i < 3; i++)
        {
            worlds.Add(GenerateRandomMap());
        }

        MapData.PlayerPosition = Vector2Int.zero;
        MapData.WorldList = new() { worlds = worlds };
    }

    private void Start()
    {
        Globals<DataManager>.Instance.CreateSnapshot();

        positionIndicator = Instantiate(positionIndicatorPrefab);

        world.onNodeClicked += OnNodeClicked;
        world.Build(random, WorldData.nodeTypes, WorldData.connections);

        positionIndicator.transform.position = CurrentNode.transform.position;
        SetPlayerPosition(PlayerPosition, true);

        var camPos = cam.transform.position;
        camPos.x = CurrentNode.transform.position.x;
        camPos.y = CurrentNode.transform.position.y;
        cam.transform.position = camPos;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragStart = (cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.position);
        }
        if (Input.GetMouseButton(1))
        {
            if (dragStart is (Vector2, Vector2) startPos)
            {
                Vector2 dragEnd = cam.ScreenToWorldPoint(Input.mousePosition);
                var delta = dragEnd - startPos.Item1;
                Vector2 newPos = startPos.Item2 - delta;
                newPos = Vector2.Max(newPos, world.BoundingBox.min);
                newPos = Vector2.Min(newPos, world.BoundingBox.max);

                var camPos = cam.transform.position;
                camPos.x = newPos.x;
                camPos.y = newPos.y;
                cam.transform.position = camPos;

                dragStart = (cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.position);
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            dragStart = null;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            print("Debug Teleport");
            selectableNodes = new(world.Nodes);

            foreach (var node in world.Nodes)
            {
                node.SetSelected(selectableNodes.Contains(node));
            }
        }
    }

    private void OnNodeClicked(Node node)
    {
        if (!selectableNodes.Contains(node)) return;

        SetPlayerPosition(node.Position, false);
        var sceneType = world.GetSceneType(DataManager.MapData.PlayerPosition);

        this.TweenDelayedAction(() =>
        {
            Globals<RunManager>.Instance.LoadScene(sceneType);
        }, 0.3f).RunNew();
    }

    private void SetPlayerPosition(Vector2Int position, bool allowSelection)
    {
        DataManager.MapData.PlayerPosition = position;
        positionIndicator.AnimateMove(CurrentNode);

        if (allowSelection) selectableNodes = world.GetConnectedNodes(position);
        else selectableNodes.Clear();

        var curNodeType = WorldData.nodeTypes[position.y][position.x];

        var difficulty = Mathf.Clamp01((float)(position.y - 1) / (WorldData.connections.Count - 3));
        var nodeType = curNodeType.FunctionalType;
        DataManager.MapData.CurrentNodeInfo = new(difficulty, nodeType, curNodeType.sceneSeed);

        foreach (var node in world.Nodes)
        {
            node.SetSelected(selectableNodes.Contains(node));
        }
    }

    private WorldLayout GenerateRandomMap()
    {
        var connections = MapConnectionGenerator.GenerateRandomConnections(random);
        var data = new WorldLayout
        {
            nodeTypes = new List<List<ExtendedNodeType>>(),
            connections = connections
        };

        for (int i = 0; i < connections.Count; i++)
        {
            var layer = connections[i];
            var typeLayer = new List<ExtendedNodeType>();
            data.nodeTypes.Add(typeLayer);

            for (int j = 0; j < layer.Count; j++)
            {
                var type = ExtendedNodeType.Fight(random.GenSeed());
                typeLayer.Add(type);
                data.nodeTypes[i][j] = type;
            }
        }
        CreateNodeTypes(data);
        return data;
    }

    private void CreateNodeTypes(WorldLayout data)
    {
        int shopCount = random.IntRange(1, 3);
        int eliteCount = random.IntRange(2, 5);
        int chestCount = random.IntRange(2, 5);
        int eventCount = random.IntRange(1, 5);

        data.nodeTypes[0][0] = ExtendedNodeType.Spawn(data.nodeTypes[0][0].sceneSeed);
        data.nodeTypes[^1][0] = ExtendedNodeType.Boss(data.nodeTypes[^1][0].sceneSeed);

        void PickRandoms(int count, int minLayer, NodeType type)
        {
            var choices = new List<Vector2Int>();
            for (int i = minLayer; i < data.nodeTypes.Count; i++)
            {
                for (int j = 0; j < data.nodeTypes[i].Count; j++)
                {
                    if (data.nodeTypes[i][j].appearanceType == NodeType.Fight)
                    {
                        choices.Add(new Vector2Int(j, i));
                    }
                }
            }

            int placeCount = Mathf.Min(count, choices.Count);
            Debug.Log($"Placing {placeCount} {type} nodes");

            for (int i = 0; i < placeCount; i++)
            {
                var index = random.IntRange(0, choices.Count);
                var choice = choices[index];
                var seed = data.nodeTypes[choice.y][choice.x].sceneSeed;
                var extendedType = type switch
                {
                    NodeType.Shop => ExtendedNodeType.Shop(seed),
                    NodeType.HardFight => ExtendedNodeType.HardFight(seed),
                    NodeType.Chest => ExtendedNodeType.Chest(seed),
                    NodeType.Event => ExtendedNodeType.RandomEvent(seed, random),
                    _ => throw new System.ArgumentOutOfRangeException()
                };

                data.nodeTypes[choice.y][choice.x] = extendedType;
                choices.RemoveAt(index);
            }
        }

        PickRandoms(shopCount, 2, NodeType.Shop);
        PickRandoms(eliteCount, 2, NodeType.HardFight);
        PickRandoms(chestCount, 1, NodeType.Chest);
        PickRandoms(eventCount, 1, NodeType.Event);
    }
}

public struct WorldList
{
    public List<WorldLayout> worlds;

    public bool HasWorld(int index)
    {
        return index >= 0 && index < worlds.Count;
    }
}

public struct CurrentNodeInfo
{
    public float difficulty;
    public NodeType nodeType;
    public int sceneSeed;

    public CurrentNodeInfo(float difficulty, NodeType nodeType, int sceneSeed)
    {
        this.difficulty = difficulty;
        this.nodeType = nodeType;
        this.sceneSeed = sceneSeed;
    }
}

public struct WorldLayout
{
    public List<List<ExtendedNodeType>> nodeTypes;
    public List<List<List<int>>> connections;
}

public static class MapConnectionGenerator
{
    public static List<List<List<int>>> GenerateRandomConnections(SeededRandom random)
    {
        List<List<List<int>>> connections = new();
        int layerCount = random.IntRangeInclusive(11, 14);

        int prevNodeCount = 0;

        for (int i = 0; i < layerCount; i++)
        {
            int nodeCount = random.IntRangeInclusive(3, 4);
            if (i == 0 || i == layerCount - 1) nodeCount = 1;
            if (i == 1 || i == layerCount - 2) nodeCount = random.IntRangeInclusive(2, 3);

            if (i > 0)
            {
                var sortedPair = (Mathf.Min(prevNodeCount, nodeCount), Mathf.Max(prevNodeCount, nodeCount));
                var options = connectionTypes[sortedPair];

                var option = options[random.IntRange(0, options.Count)];
                connections.Add(GetConnections(prevNodeCount, option, prevNodeCount > nodeCount));
            }
            prevNodeCount = nodeCount;
        }

        connections.Add(new List<List<int>>() { new() });

        return connections;
    }

    private static List<List<int>> GetConnections(int prevCount, List<(int, int)> connections, bool reversed)
    {
        var list = new List<List<int>>();

        for (int i = 0; i < prevCount; i++)
        {
            list.Add(new List<int>());
        }

        foreach (var connection in connections)
        {
            var (prevIndex, thisIndex) = reversed ? (connection.Item2, connection.Item1) : connection;
            list[prevIndex].Add(thisIndex);
        }

        return list;
    }

    public static Dictionary<(int, int), List<List<(int, int)>>> connectionTypes = BuildConnectionTypes();

    public static Dictionary<(int, int), List<List<(int, int)>>> BuildConnectionTypes()
    {
        var dict = new Dictionary<(int, int), List<List<(int, int)>>>();

        dict[(1, 1)] = new() { new() { (0, 0) } };
        dict[(1, 2)] = new() { new() { (0, 0), (0, 1) } };
        dict[(1, 3)] = new() { new() { (0, 0), (0, 1), (0, 2) } };
        dict[(1, 4)] = new() { new() { (0, 0), (0, 1), (0, 2), (0, 3) } };
        dict[(2, 2)] = new() {
            new() { (0, 0), (1, 0), (1, 1) },
            new() { (0, 0), (0, 1), (1, 1) }
        };
        dict[(2, 3)] = new() {
            new() { (0, 0), (0, 1), (1, 1), (1, 2) },
            new() { (0, 0), (0, 1), (0, 2), (1, 2) },
            new() { (0, 0), (1, 0), (1, 1), (1, 2) }
        };
        dict[(2, 4)] = new() {
            new() { (0, 0), (0, 1), (1, 2), (1, 3) },
            new() { (0, 0), (0, 1), (0, 2), (1, 2), (1, 3) },
            new() { (0, 0), (0, 1), (1, 1), (1, 2), (1, 3) },
        };
        dict[(3, 3)] = new() {
            new() { (0, 0), (0, 1), (1, 1), (1, 2), (2, 2) },
            new() { (0, 0), (1, 0), (1, 1), (1, 2), (2, 2) },
            new() { (0, 0), (0, 1), (1, 1), (2, 1), (2, 2) },
            new() { (0, 0), (1, 0), (1, 1), (2, 1), (2, 2) },
        };
        dict[(3, 4)] = new() {
            new() { (0, 0), (0, 1), (1, 1), (1, 2), (2, 2), (2, 3) },
            new() { (0, 0), (1, 0), (1, 1), (1, 2), (2, 2), (2, 3) },
            new() { (0, 0), (0, 1), (1, 1), (1, 2), (1, 3), (2, 3) },
        };
        dict[(4, 4)] = new() {
            new() { (0, 0), (0, 1), (1, 1), (2, 2), (2, 3), (3, 3) },
            new() { (0, 0), (1, 0), (1, 1), (2, 1), (2, 2), (3, 3) },
            new() { (0, 0), (0, 1), (1, 1), (1, 2), (2, 2), (3, 2), (3, 3) },
            new() { (0, 0), (1, 0), (1, 1), (2, 1), (2, 2), (3, 2), (3, 3) },
        };

        return dict;
    }
}