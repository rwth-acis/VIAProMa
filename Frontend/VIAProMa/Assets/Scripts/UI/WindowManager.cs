using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    [SerializeField] private GameObject roomMenuPrefab;
    [SerializeField] private GameObject serverStatusMenuPrefab;
    [SerializeField] private GameObject chatMenuPrefab;
    [SerializeField] private GameObject saveProjectMenuPrefab;
    [SerializeField] private GameObject loginMenuPrefab;
    [SerializeField] private GameObject connectionLinesMenuPrefab;
    [SerializeField] private GameObject testMenuPrefab;

    private void Start()
    {
        RoomMenu = (RoomMenu)InstantiateWindow(roomMenuPrefab);
        ServerStatusMenu = (ServerStatusMenu)InstantiateWindow(serverStatusMenuPrefab);
        ChatMenu = (ChatMenu)InstantiateWindow(chatMenuPrefab);
        SaveProjectWindow = (SaveProjectWindow)InstantiateWindow(saveProjectMenuPrefab);
        LoginMenu = (LoginMenu)InstantiateWindow(loginMenuPrefab);
        ConnectionLinesMenu = (ConnectionLinesMenu)InstantiateWindow(connectionLinesMenuPrefab);
        TestMenu = (TestMenu)InstantiateWindow(testMenuPrefab);
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
        window.Close();
        return window;
    }

    public RoomMenu RoomMenu { get; private set; }

    public ServerStatusMenu ServerStatusMenu { get; private set; }

    public ChatMenu ChatMenu { get; private set; }

    public SaveProjectWindow SaveProjectWindow { get; private set; }

    public LoginMenu LoginMenu { get; private set; }

    public ConnectionLinesMenu ConnectionLinesMenu { get; private set; }

    public TestMenu TestMenu { get; private set; }
}
