using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private RectTransform pauseMenu;
    [SerializeField] private BetterButton closeButton, resetSettingsButton, mainMenuButton;

    private void Awake()
    {
        pauseMenu.gameObject.SetActive(false);
        closeButton.AddClickListener(OnCloseButtonClicked);
        resetSettingsButton.AddClickListener(OnResetSettingsButtonClicked);
        mainMenuButton.AddClickListener(OnMainMenuButtonClicked);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.gameObject.SetActive(true);
            Signals.Get<PauseMenuIsOpen>().Dispatch(true);
        }
    }

    private void OnCloseButtonClicked()
    {
        pauseMenu.gameObject.SetActive(false);
        Signals.Get<PauseMenuIsOpen>().Dispatch(false);
    }

    private void OnResetSettingsButtonClicked()
    {
        DataManger.SettingsData.Reset();
    }

    private void OnMainMenuButtonClicked()
    {
        Globals<RunManager>.Instance.LoadScene(SceneType.MainMenu);
    }
}
