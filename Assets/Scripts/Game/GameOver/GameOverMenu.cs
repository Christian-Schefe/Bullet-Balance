using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private SceneObject mapScene, menuScene;
    [SerializeField] private BetterButton restartButton, toMenuButton;
    [SerializeField] private SceneTransition transistion;

    private void Awake()
    {
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
