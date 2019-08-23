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
    [SerializeField] private Interactable roomButton;
    [SerializeField] private TextMeshPro roomButtonText;
    [SerializeField] private Interactable chatButton;
    [SerializeField] private Interactable microphoneButton;

    private void Awake()
    {
        if (roomButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomButton));
        }
        if (roomButtonText == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomButtonText));
        }
        if (chatButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(chatButton));
        }
        if (microphoneButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(microphoneButton));
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
        roomButton.Enabled = PhotonNetwork.IsConnected;
        chatButton.Enabled = PhotonNetwork.IsConnected;
        microphoneButton.Enabled = PhotonNetwork.IsConnected;
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
            WindowManager.Instance.RoomMenu.Open(roomButton.transform.position - 0.1f * transform.forward, transform.localEulerAngles);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}
