using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FoldController))]
public class MainMenu : MonoBehaviour
{
    //[SerializeField] private Interactable 
    [SerializeField] private TextMeshPro roomButtonText;

    private void Awake()
    {
        if (roomButtonText == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomButtonText));
        }
    }

    private void OnEnable()
    {
        LobbyManager.Instance.LobbyJoinStatusChanged += OnLobbyStatusChanged;
        Launcher.Instance.ConnectionStatusChanged += OnConnectionStatusChanged;
    }

    private void OnDisable()
    {
        LobbyManager.Instance.LobbyJoinStatusChanged -= OnLobbyStatusChanged;
        Launcher.Instance.ConnectionStatusChanged -= OnConnectionStatusChanged;
    }

    private void OnConnectionStatusChanged(object sender, EventArgs e)
    {

    }

    private void OnLobbyStatusChanged(object sender, EventArgs e)
    {
        if (PhotonNetwork.InLobby)
        {
            roomButtonText.text = "Rooms";
        }
        else
        {
            roomButtonText.text = "Leave Room";
        }
    }

    public void ShowServerStatusMenu()
    {
        WindowManager.Instance.ServerStatusMenu.Open(transform.localPosition + 0.1f * transform.forward, transform.localEulerAngles);
    }

    public void RoomButtonClicked()
    {
        // if in lobby: show room menu
        // otherwise: leave the current room
        if (PhotonNetwork.InLobby)
        {
            WindowManager.Instance.RoomMenu.Open(transform.localPosition + 0.1f * transform.forward, transform.localEulerAngles);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
