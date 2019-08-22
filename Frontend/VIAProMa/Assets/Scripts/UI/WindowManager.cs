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

    protected override void Awake()
    {
        base.Awake();
        roomMenuInstance = (RoomMenu)InstantiateWindow(roomMenuPrefab);
        serverStatusMenuInstance = (ServerStatusMenu)InstantiateWindow(serverStatusMenuPrefab);
    }

    private IWindow InstantiateWindow(GameObject prefab)
    {
        GameObject windowInstance = Instantiate(prefab, transform);
        return windowInstance.GetComponentInChildren<IWindow>();
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
