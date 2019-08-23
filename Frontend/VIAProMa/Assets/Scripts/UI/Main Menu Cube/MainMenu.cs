using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FoldController))]
public class MainMenu : MonoBehaviour
{
    public void ShowServerStatusMenu()
    {
        WindowManager.Instance.ServerStatusMenu.Open(transform.localPosition + 0.1f * transform.forward, transform.localEulerAngles);
    }

    public void ShowRoomMenu()
    {
        WindowManager.Instance.RoomMenu.Open(transform.localPosition + 0.1f * transform.forward, transform.localEulerAngles);
    }
}
