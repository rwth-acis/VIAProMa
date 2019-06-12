using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    string gameVersion = "1";

    public static Launcher Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of " + nameof(Launcher));
        }
        Instance = this;
    }

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster()");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("OnDisconnected()");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Client is now in the room " + PhotonNetwork.CurrentRoom.Name);

        PhotonNetwork.LoadLevel(1);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Client is now in the lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room join failed\n" + returnCode + ": " + message);
    }
}
