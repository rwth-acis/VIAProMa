using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ParticipantListTest : MonoBehaviour 
{
    public GameObject SecondPlayerDummy;
    public GameObject ThirdPlayerDummy;
    public GameObject ListWindow;

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
        SecondPlayerDummy.SetActive(true);
        ThirdPlayerDummy.SetActive(true);
        ListWindow.GetComponent<ParticipantListManager>().TestCall();
    }

    public void Connect()
    {
        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom("TestRoom");
            Debug.Log("Joined room succesfully");
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Could not connect to server!");
            }
        }
    }

    public void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
}
