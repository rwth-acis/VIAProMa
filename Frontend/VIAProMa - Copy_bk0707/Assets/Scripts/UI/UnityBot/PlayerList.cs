using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.UI;
using Photon.Pun;
using System;
using UnityEngine;

public class PlayerList : MonoBehaviour, IWindow
{

    [SerializeField] GameObject playerListItem;
    [SerializeField] private string notificationPrefab;
    //[SerializeField] GameObject _gameObject;
    bool IWindow.WindowEnabled
    {
        get;
        set;
    }

    bool IWindow.WindowOpen
    {
        get
        {
            return gameObject.activeSelf;
        }
    }

    public event EventHandler WindowClosed;

    public void ListPlayer()
    {
        playerListItem.GetComponent<PlayerListItem>().GetPlayerList();
    }

    public void DoneButtonOnClick()
    {
        Debug.Log(PlayerListItem.playerName);
        Vector3 targetPosition = transform.position - 1f * transform.right;
        targetPosition.y = 1f;
        PhotonNetwork.Instantiate(notificationPrefab, targetPosition, Quaternion.identity, 0);
        Close();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        gameObject.SetActive(true);

    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.position = position;
        transform.eulerAngles = eulerAngles;
    }
}
