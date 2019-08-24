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
    [Header("UI Elements")]
    [SerializeField] private Interactable avatarConfigurationButton;
    [SerializeField] private Interactable roomButton;
    [SerializeField] private TextMeshPro roomButtonText;
    [SerializeField] private Interactable chatButton;
    [SerializeField] private Interactable microphoneButton;

    [Header("References")]
    [SerializeField] private GameObject issueShelfPrefab;
    [SerializeField] private GameObject avatarConfiguratorPrefab;

    private void Awake()
    {
        if (avatarConfigurationButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarConfigurationButton));
        }
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

        if (issueShelfPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueShelfPrefab));
        }
        if (avatarConfiguratorPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarConfiguratorPrefab));
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

    public void ShowIssueShelf()
    {
        Vector3 position = transform.position - 0.15f * transform.forward;
        position.y = 0f;
        Quaternion rotation = transform.rotation;
        GameObject issueShelf = ResourceManager.Instance.NetworkInstantiate(issueShelfPrefab, position, rotation);
    }

    public void ShowAvatarConfiguration()
    {
        GameObject avatarConfigurator = Instantiate(avatarConfiguratorPrefab);
        avatarConfigurator.transform.localPosition = avatarConfigurationButton.transform.position - 0.15f * transform.forward;
        avatarConfigurator.transform.localRotation = transform.localRotation;
    }

    public void ShowServerStatusMenu()
    {
        WindowManager.Instance.ServerStatusMenu.Open(transform.position - 0.1f * transform.forward, transform.localEulerAngles);
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
