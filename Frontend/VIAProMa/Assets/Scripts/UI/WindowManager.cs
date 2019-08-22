using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    [SerializeField] private RoomMenu roomMenu;
    [SerializeField] private ServerStatusMenu serverStatusMenu;

    public RoomMenu RoomMenu
    {
        get => roomMenu;
    }

    public ServerStatusMenu ServerStatusMenu
    {
        get => serverStatusMenu;
    }
}
