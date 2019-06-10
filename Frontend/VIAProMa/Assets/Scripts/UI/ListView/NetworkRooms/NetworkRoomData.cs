using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkRoomData : IListViewItemData
{
    public RoomInfo RoomInfo { get; private set; }

    public bool IsFull { get { return RoomInfo.MaxPlayers != 0 && RoomInfo.PlayerCount == RoomInfo.MaxPlayers; } }

    public NetworkRoomData(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
    }
}
