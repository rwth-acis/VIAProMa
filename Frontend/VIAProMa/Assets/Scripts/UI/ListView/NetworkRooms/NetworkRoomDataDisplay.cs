using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetworkRoomDataDisplay : DataDisplay<NetworkRoomData>
{
    [SerializeField] private TextMeshPro roomNameLabel;
    [SerializeField] private TextMeshPro memberNumberLabel;

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
        }
        else
        {
            roomNameLabel.text = "NULL";
        }
    }
}
