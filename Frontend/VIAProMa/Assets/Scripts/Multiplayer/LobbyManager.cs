using HoloToolkit.Unity;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the logic for managing the network lobby
/// For instance, this class administers the available rooms
/// </summary>
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public event EventHandler LobbyJoinStatusChanged;
    public event EventHandler RoomListChanged;

    public static LobbyManager Instance { get; private set; }

    public List<NetworkRoomData> Rooms { get; private set; } = new List<NetworkRoomData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of " + nameof(LobbyManager));
        }
        Instance = this;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Rooms.Clear();
        foreach (RoomInfo roomInfo in roomList)
        {
            if (!roomInfo.RemovedFromList)
            {
                Rooms.Add(new NetworkRoomData(roomInfo));
            }
        }

        RoomListChanged?.Invoke(this, EventArgs.Empty);
    }

    public override void OnJoinedRoom()
    {
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
