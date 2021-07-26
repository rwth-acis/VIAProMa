using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the voice transmission
/// </summary>
public class VoiceRecorderManager : MonoBehaviour
{
    /// <summary>
    /// Initializes the primary voice recorder
    /// </summary>
    private void Start()
    {
        PhotonVoiceNetwork.Instance.PrimaryRecorder.Init(PhotonVoiceNetwork.Instance);
        Debug.Log("Voice Recorder initialized");
    }
}
