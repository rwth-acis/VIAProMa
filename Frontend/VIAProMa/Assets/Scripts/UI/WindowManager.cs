using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    [SerializeField] private GameObject roomMenuPrefab;
    [SerializeField] private GameObject serverStatusMenuPrefab;

    private RoomMenu roomMenuInstance;
    private ServerStatusMenu serverStatusMenuInstance;

    private void Start()
    {
        roomMenuInstance = (RoomMenu)InstantiateWindow(roomMenuPrefab);
        serverStatusMenuInstance = (ServerStatusMenu)InstantiateWindow(serverStatusMenuPrefab);
    }

    private IWindow InstantiateWindow(GameObject prefab)
    {
        GameObject windowInstance = Instantiate(prefab, transform);
        IWindow window = windowInstance.GetComponentInChildren<IWindow>();
        if (window == null)
        {
            Destroy(windowInstance);
            Debug.LogError("Window Manager tried to create a prefab without a window component.\nAdd the window component to the prefab " + prefab.name);
            return null;
        }
        windowInstance.SetActive(false);
        return window;
    }

    public RoomMenu RoomMenu
    {
        get => roomMenuInstance;
    }

    public ServerStatusMenu ServerStatusMenu
    {
        get => serverStatusMenuInstance;
    }
}
