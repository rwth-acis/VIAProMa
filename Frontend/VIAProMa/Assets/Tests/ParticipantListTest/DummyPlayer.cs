using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class DummyPlayer : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";
    private static System.Random random = new System.Random();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom("TestRoom");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster - Secondary Player");
        PhotonNetwork.JoinRoom("TestRoom");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed. Creating new room. - Secondary Player");
        PhotonNetwork.CreateRoom("TestRoom");
        PhotonNetwork.JoinRoom("TestRoom");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room- Secondary Player");
        PhotonNetwork.NickName = RandomString(5);
        Debug.Log("Numebr of Players currently in the Room: "+ PhotonNetwork.PlayerList.Length);
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
