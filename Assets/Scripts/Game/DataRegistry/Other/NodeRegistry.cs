using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeRegistry", menuName = "Game/NodeRegistry")]
public class NodeRegistry : SimpleRegistry<NodeType, NodeRegistry.Entry>
{
    [Serializable]
    public class Entry : IRegistryEntry<NodeType>
    {
        public NodeType type;
        public Sprite icon;
        public Color color;
        public SceneType scene;

        public NodeType Key => type;
    }
}

public enum NodeType
{
    Spawn, Fight, HardFight, Shop, Boss, Chest, Event
}

public class ExtendedNodeType
{
    public NodeType appearanceType;
    public NodeType? functionalType;
    public int sceneSeed;

    public NodeType FunctionalType => functionalType ?? appearanceType;

    public ExtendedNodeType()
    {
        appearanceType = NodeType.Fight;
        functionalType = null;
    }

    public ExtendedNodeType(int seed, NodeType appearanceType, NodeType? functionalType = null)
    {
        this.sceneSeed = seed;
        this.appearanceType = appearanceType;
        this.functionalType = functionalType;
    }

    public override string ToString()
    {
        return $"{appearanceType}" + (functionalType.HasValue ? $" ({functionalType})" : "") + $" [{sceneSeed}]";
    }

    public static ExtendedNodeType Spawn(int seed) => new(seed, NodeType.Spawn);
    public static ExtendedNodeType Fight(int seed) => new(seed, NodeType.Fight);
    public static ExtendedNodeType HardFight(int seed) => new(seed, NodeType.HardFight);
    public static ExtendedNodeType Shop(int seed) => new(seed, NodeType.Shop);
    public static ExtendedNodeType Boss(int seed) => new(seed, NodeType.Boss);
    public static ExtendedNodeType Chest(int seed) => new(seed, NodeType.Chest);
    public static ExtendedNodeType RandomEvent(int seed, SeededRandom random) => new(seed, NodeType.Event, random.Probability(0.5f) ? null : random.Choose(new List<NodeType>() { NodeType.Fight, NodeType.HardFight, NodeType.Chest, NodeType.Shop }));
}
