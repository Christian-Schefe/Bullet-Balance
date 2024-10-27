using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private BetterButton continueButton, startRunButton, quitButton;

    private void Awake()
    {
        startRunButton.AddClickListener(OnPressStartRun);
        quitButton.AddClickListener(OnPressQuit);

        continueButton.AddClickListener(OnPressContinue);
        continueButton.SetInteractable(Globals<DataManger>.Instance.CanContinueRun());
    }

    private void OnPressContinue()
    {
        if (!Globals<DataManger>.Instance.CanContinueRun()) return;

        Globals<RunManager>.Instance.ContinueRun();
    }

    private void OnPressStartRun()
    {
        Globals<RunManager>.Instance.StartNewRun();
    }

    private void OnPressQuit()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
        Application.Quit();
    }
}
