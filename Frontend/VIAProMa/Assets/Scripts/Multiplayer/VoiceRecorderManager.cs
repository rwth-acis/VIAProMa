using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceRecorderManager : MonoBehaviour
{
    private void Start()
    {
        PhotonVoiceNetwork.Instance.PrimaryRecorder.Init(PhotonVoiceNetwork.Instance.VoiceClient);
        Debug.Log("Voice Recorder initialized");
    }
}
