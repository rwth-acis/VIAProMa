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
    /// <summary>
    /// Event which is invoked if this client has changed the lobby status, e.g. joined or left the lobby
    /// </summary>
    public event EventHandler LobbyJoinStatusChanged;
    /// <summary>
    /// Event which is invoked if the room list is changed
    /// </summary>
    public event EventHandler RoomListChanged;

    /// <summary>
    /// Singleton instance
    /// Does not use the singleton class since we need to derive from MonoBehaviourPunCallbacks
    /// </summary>
    /// <value></value>
    public static LobbyManager Instance { get; private set; }

    /// <summary>
    /// The available rooms in the lobby
    /// </summary>
    /// <typeparam name="NetworkRoomData">Data about the room, e.g. if it is closed</typeparam>
    /// <returns>The available rooms</returns>
    public List<NetworkRoomData> Rooms { get; private set; } = new List<NetworkRoomData>();

    /// <summary>
    /// Initializes the singleton instance
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of " + nameof(LobbyManager));
        }
        Instance = this;
    }

    /// <summary>
    /// Called by Photon if the client joins the lobby
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called by Photon if the client leaves the lobby
    /// </summary>
    public override void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Called by Photon if the lobby statistics are changed
    /// </summary>
    /// <param name="lobbyStatistics"></param>
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    /// <summary>
    /// Called by Photon if the room list is changed
    /// Keeps track of the room list
    /// </summary>
    /// <param name="roomList">The new room list</param>
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

    /// <summary>
    /// Called by Photon if the client leaves the lobby and joins a room
    /// It is necessary to react to this in order to monitor all possible ways for leaving the lobby
    /// OnLeftLobby is not called if the client joins a room
    /// </summary>
    public override void OnJoinedRoom()
    {
        LobbyJoinStatusChanged?.Invoke(this, EventArgs.Empty);
    }
}
