using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsWindow : MonoBehaviour
{
    [SerializeField] private BetterButton openButton, closeButton;
    [SerializeField] private RectTransform settingsMenu;

    private void Awake()
    {
        openButton.onClick.AddListener(OnPressOpen);
        closeButton.onClick.AddListener(OnPressClose);
        settingsMenu.gameObject.SetActive(false);
    }

    private void OnPressClose()
    {
        settingsMenu.gameObject.SetActive(false);
    }

    public void OnPressOpen()
    {
        settingsMenu.gameObject.SetActive(true);
    }
}
