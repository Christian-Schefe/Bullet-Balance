using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    [SerializeField] private SceneRegistry sceneRegistry;

    private void Awake()
    {
        Globals<RunManager>.RegisterOrDestroy(this);
    }

    public void ContinueRun()
    {
        Globals<DataManager>.Instance.ContinueRun();
        LoadScene(SceneType.Map);
    }

    private void Update()
    {
        var currentScene = sceneRegistry.CurrentScene;
        if (currentScene == SceneType.MainMenu || currentScene == SceneType.GameOver) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            LoadScene(SceneType.Map);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadScene(SceneType.Reward);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameOver();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            DataManager.PlayerData.Gold += 10;
        }
    }

    public void StartNewRun()
    {
        Globals<DataManager>.Instance.StartNewRun();
        LoadScene(SceneType.Map);
    }

    public void GameOver()
    {
        DataManager.RunData.RunState = RunState.GameOver;
        LoadScene(SceneType.GameOver);
    }

    public void Win()
    {
        DataManager.RunData.RunState = RunState.Win;
        LoadScene(SceneType.GameOver);
    }

    public void LoadScene(SceneType sceneType)
    {
        var sceneObject = sceneRegistry[sceneType].sceneObject;
        Globals<SceneTransition>.Instance.TransitionLoadScene(sceneObject);
    }
}
