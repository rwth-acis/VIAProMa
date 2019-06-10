using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkRoomDataDisplay : DataDisplay<NetworkRoomData>
{
    [SerializeField] private TextMeshPro roomNameLabel;
    [SerializeField] private TextMeshPro memberNumberLabel;
    [SerializeField] private GameObject lockIcon;

    private Interactable button;

    private void Awake()
    {
        if (roomNameLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(roomNameLabel));
        }
        if (memberNumberLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(memberNumberLabel));
        }
        if (lockIcon == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(lockIcon));
        }

        button = GetComponent<Interactable>();
        if (button == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), gameObject);
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        if (content != null && content.RoomInfo != null)
        {
            roomNameLabel.text = content.RoomInfo.Name;
            if (content.RoomInfo.MaxPlayers == 0) // max players has not been set
            {
                if (content.RoomInfo.PlayerCount == 1)
                {
                    memberNumberLabel.text = content.RoomInfo.PlayerCount + "\nmember";
                }
                else
                {
                    memberNumberLabel.text = content.RoomInfo.PlayerCount + "\nmembers";
                }
            }
            else
            {
                memberNumberLabel.text = content.RoomInfo.PlayerCount + "/" + content.RoomInfo.MaxPlayers + "\nmembers";
            }

            bool roomOpen = content.RoomInfo.IsOpen && !content.IsFull;
            button.Enabled = roomOpen;
            lockIcon.SetActive(!roomOpen);
        }
        else
        {
            roomNameLabel.text = "NULL";
        }
    }
}
