using System;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private InventoryWindow inventoryWindow;

    private IWindow openWindow;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Open(pauseMenu);
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            Open(inventoryWindow);
        }
    }

    public void Open(IWindow window)
    {
        openWindow?.Close();
        if (window == openWindow)
        {
            openWindow = null;
        }
        else
        {
            window.Open(() => OnClose(window));
            openWindow = window;
        }
    }

    private void OnClose(IWindow window)
    {
        window.Close();
        openWindow = null;
    }
}

public interface IWindow
{
    public void Open(Action closeCallback);
    public void Close();
}