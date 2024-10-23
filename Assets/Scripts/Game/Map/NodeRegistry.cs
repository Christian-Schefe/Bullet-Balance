using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeRegistry", menuName = "Game/NodeRegistry")]
public class NodeRegistry : Registry<NodeType, NodeRegistry.Entry>
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
