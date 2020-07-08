using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class ParticipantListTest : MonoBehaviourPunCallbacks
{

    public GameObject ListWindow;

    private static System.Random random = new System.Random();
    private string gameVersion = "1";

    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinOrCreateRoom("TestRoom", null, null);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", null, null);

    }

    public void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room");
        PhotonNetwork.NickName = RandomString(5);
        ListWindow.GetComponent<ParticipantListManager>().TestCall();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed. Creating new room");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", null, null);
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
    }

}
