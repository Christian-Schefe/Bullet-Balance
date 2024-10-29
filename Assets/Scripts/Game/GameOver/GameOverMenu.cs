using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sceneTitle;

    [SerializeField] private SceneReference mapScene, menuScene;
    [SerializeField] private BetterButton restartButton, toMenuButton;
    [SerializeField] private SceneTransition transistion;

    private void Awake()
    {
        var runState = DataManger.RunData.RunState;
        sceneTitle.text = runState == RunState.Win ? "You Win!" : "Game Over";

        restartButton.AddClickListener(OnPressRestart);
        toMenuButton.AddClickListener(OnPressToMenu);
    }

    private void OnPressRestart()
    {
        Globals<RunManager>.Instance.StartNewRun();
    }

    private void OnPressToMenu()
    {
        transistion.TransitionLoadScene(menuScene);
    }
}
