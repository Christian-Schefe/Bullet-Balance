using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour, IWindow
{
    [SerializeField] private RectTransform pauseMenu;
    [SerializeField] private BetterButton closeButton, resetSettingsButton, mainMenuButton;

    private Action closeCallback;

    private void Awake()
    {
        pauseMenu.gameObject.SetActive(false);
        closeButton.AddClickListener(OnCloseButtonClicked);
        resetSettingsButton.AddClickListener(OnResetSettingsButtonClicked);
        mainMenuButton.AddClickListener(OnMainMenuButtonClicked);
    }

    public void SetOpen(bool open)
    {
        pauseMenu.gameObject.SetActive(open);
        Signals.Get<PauseMenuIsOpen>().Dispatch(open);
    }

    private void OnCloseButtonClicked()
    {
        closeCallback?.Invoke();
    }

    private void OnResetSettingsButtonClicked()
    {
        DataManager.SettingsData.Reset();
    }

    private void OnMainMenuButtonClicked()
    {
        Globals<RunManager>.Instance.LoadScene(SceneType.MainMenu);
    }

    public void Open(Action closeCallback)
    {
        SetOpen(true);
        this.closeCallback = closeCallback;
    }

    public void Close()
    {
        SetOpen(false);
    }
}
