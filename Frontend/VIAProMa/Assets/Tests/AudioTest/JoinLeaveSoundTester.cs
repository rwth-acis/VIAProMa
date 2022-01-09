using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinLeaveSoundTester : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("JoinLeaveSoundTest", null, null);
            }
        }

        if (PhotonNetwork.IsConnected && Input.GetKeyDown(KeyCode.J))
        {
            AudioManager.instance.PlayLoginSound(transform.position);
        }

        if (PhotonNetwork.IsConnected && Input.GetKeyDown(KeyCode.L))
        {
            AudioManager.instance.PlayLogoffSound(transform.position);
        }


    }
}
