using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceChatTester : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("VoiceTest", null, null);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("Toggling voice");
                PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = !PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled;
            }
        }
    }
}
