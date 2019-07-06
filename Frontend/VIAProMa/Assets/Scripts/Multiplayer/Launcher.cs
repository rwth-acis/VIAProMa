using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The launcher's task is to connect to the Photon services and to join a lobby
/// This script focuses on monitoring the connection and joining the lobby if necessary
/// Lobby Management can be found in the LobbyManager class and network logic inside a room is handled by MultiplayerManager
/// </summary>
public class Launcher : MonoBehaviourPunCallbacks
{
    string gameVersion = "1"; // this will be updated if breaking changes are made to the network behavior of the client

    public static Launcher Instance { get; private set; } // this is a singleton, but we cannot use the singleton class since it also inherits from PunCallbacks

    /// <summary>
    /// Handles the initialization
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of " + nameof(Launcher));
        }
        Instance = this;
    }

    /// <summary>
    /// Initiates a connection attempt to the server
    /// </summary>
    private void Start()
    {
        Connect(); // try to connect to the server
    }

    /// <summary>
    /// Connects to the server
    /// If the connection already exists, we proceed and go to the lobby
    /// </summary>
    private void Connect()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("now the offline mode is " + PhotonNetwork.OfflineMode);
            PhotonNetwork.NickName = "Guest" + Random.Range(0, 1000);
        }
    }

    /// <summary>
    /// Called when the client has connected to the server
    /// Client can now proceed and join a lobby
    /// </summary>
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
        if (!PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.JoinRoom("offline");
        }
    }

    /// <summary>
    /// Called when the client disconnects from the server
    /// </summary>
    /// <param name="cause">The cause for the disconnect</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected()" + cause.ToString());
    }

    /// <summary>
    /// Called when the client joins the lobby
    /// </summary>
    public override void OnJoinedLobby()
    {
        Debug.Log("Client is now in the lobby");
    }

    /// <summary>
    /// Called when the client leaves the lobby
    /// </summary>
    public override void OnLeftLobby()
    {
        Debug.Log("Client left the lobby");
    }
}
