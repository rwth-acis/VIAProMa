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

    private FoldController foldController;

    // instances:
    private GameObject issueShelfInstance;
    private GameObject avatarConfiguratorInstance;

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

        foldController = gameObject.GetComponent<FoldController>();
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
        Vector3 targetPosition = transform.position - 1f * transform.right;
        targetPosition.y = 0f;
        NetworkInstantiateControl(issueShelfPrefab, ref issueShelfInstance, targetPosition);
        foldController.FoldCube();
    }

    public void ShowAvatarConfiguration()
    {
        InstantiateControl(
            avatarConfiguratorPrefab, 
            ref avatarConfiguratorInstance,
            transform.position - 1f * transform.right);
        foldController.FoldCube();
    }

    public void ShowServerStatusMenu()
    {
        WindowManager.Instance.ServerStatusMenu.Open(transform.position - 0.4f * transform.right, transform.localEulerAngles);
        foldController.FoldCube();
    }

    public void RoomButtonClicked()
    {
        // if in lobby: show room menu
        // otherwise: leave the current room
        if (PhotonNetwork.InLobby)
        {
            WindowManager.Instance.RoomMenu.Open(roomButton.transform.position - 0.4f * transform.right, transform.localEulerAngles);
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }
        foldController.FoldCube();
    }

    private void InstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition)
    {
        Quaternion targetRotation = transform.rotation;

        if (instance != null)
        {
            instance.transform.position = targetPosition;
            instance.transform.rotation = targetRotation;
        }
        else
        {
            instance = GameObject.Instantiate(prefab, targetPosition, targetRotation);
        }
    }

    private void NetworkInstantiateControl(GameObject prefab, ref GameObject instance, Vector3 targetPosition)
    {
        Quaternion targetRotation = transform.rotation;

        if (instance != null)
        {
            instance.transform.position = targetPosition;
            instance.transform.rotation = targetRotation;
        }
        else
        {
            instance = ResourceManager.Instance.NetworkInstantiate(prefab, targetPosition, targetRotation);
        }
    }
}
