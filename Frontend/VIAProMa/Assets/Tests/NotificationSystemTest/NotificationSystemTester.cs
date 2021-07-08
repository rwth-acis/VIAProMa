using i5.VIAProMa.Multiplayer.Chat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSystemTester : MonoBehaviour
{
    public string message;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            NotificationSystem.Instance.ShowMessage(message);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            NotificationSystem.Instance.HideMessage();
        }
    }
}
