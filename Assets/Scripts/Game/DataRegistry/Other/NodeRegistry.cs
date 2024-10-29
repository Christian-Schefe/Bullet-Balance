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

    public NodeType FunctionalType => functionalType ?? appearanceType;

    public ExtendedNodeType()
    {
        appearanceType = NodeType.Fight;
        functionalType = null;
    }

    public ExtendedNodeType(NodeType appearanceType, NodeType? functionalType = null)
    {
        this.appearanceType = appearanceType;
        this.functionalType = functionalType;
    }

    public static bool operator ==(ExtendedNodeType a, ExtendedNodeType b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(ExtendedNodeType a, ExtendedNodeType b)
    {
        return !a.Equals(b);
    }

    public override bool Equals(object obj)
    {
        return obj is ExtendedNodeType other && appearanceType == other.appearanceType && functionalType == other.functionalType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(appearanceType, functionalType);
    }

    public override string ToString()
    {
        return $"{appearanceType}" + (functionalType.HasValue ? $" ({functionalType})" : "");
    }

    public static ExtendedNodeType Spawn => new(NodeType.Spawn);
    public static ExtendedNodeType Fight => new(NodeType.Fight);
    public static ExtendedNodeType HardFight => new(NodeType.HardFight);
    public static ExtendedNodeType Shop => new(NodeType.Shop);
    public static ExtendedNodeType Boss => new(NodeType.Boss);
    public static ExtendedNodeType Chest => new(NodeType.Chest);
    public static ExtendedNodeType Event => new(NodeType.Event);
    public static ExtendedNodeType RandomEvent(SeededRandom random) => new(NodeType.Event, random.Probability(0.5f) ? null : random.Choose(new List<NodeType>() { NodeType.Fight, NodeType.HardFight, NodeType.Chest, NodeType.Shop }));
}
