using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneRegistry", menuName = "Game/SceneRegistry")]
public class SceneRegistry : SimpleRegistry<SceneType, SceneRegistry.Entry>
{
    public SceneType CurrentScene
    {
        get
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var sceneName = scene.name;
            return new List<Entry>(Lookup.Values).Find(e => e.sceneObject.sceneName == sceneName).Key;
        }
    }

    [System.Serializable]
    public class Entry : IRegistryEntry<SceneType>
    {
        public SceneReference sceneObject;
        public SceneType type;

        public SceneType Key => type;
    }
}

public enum SceneType
{
    MainMenu,
    Map,
    Fight,
    GameOver,
    Reward,
    Shop,
    Event,
    Chest,
}
