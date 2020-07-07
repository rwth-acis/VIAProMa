using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DummyPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    { 
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.JoinRoom("Test");
    }
}
