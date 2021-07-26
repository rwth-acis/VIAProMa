using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.UI;
using i5.VIAProMa.UI.UnityBot;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using UnityEngine;

public class PlayerList : MonoBehaviour, IWindow
{

    [SerializeField] GameObject playerListItem;
    [SerializeField] private string notificationPrefab;
    [SerializeField] private InteractableToggleCollection roleToggles;
    public static bool isShare = false;
    public static PermissionRoles role;
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

    private void Awake()
    {
        if (roleToggles == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roleToggles));
        }
    }

    public void ListPlayer()
    {
        playerListItem.GetComponent<PlayerListItem>().GetPlayerList();
    }

    public void PermissionRoleChanged()
    {
        //PermissionManager.Instance.PermissionRole = (PermissionRoles)roleToggles.CurrentIndex;
        Debug.Log((PermissionRoles)roleToggles.CurrentIndex);
    }

    public void DoneButtonOnClick()
    {
        Debug.Log(PlayerListItem.playerName);
        /*Vector3 targetPosition = transform.position;// - 1f * transform.right;
        targetPosition.z = 2f;
        targetPosition.y = 1f;
        PhotonNetwork.Instantiate(notificationPrefab, targetPosition, Quaternion.identity, 0);*/
        role = (PermissionRoles)roleToggles.CurrentIndex;
        isShare = true;
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
