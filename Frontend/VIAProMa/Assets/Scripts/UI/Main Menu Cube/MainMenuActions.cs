using Microsoft.MixedReality.Toolkit.UI;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuActions : MonoBehaviour
{
    [SerializeField] private Interactable microphoneButton;

    public void ToggleMicrophone()
    {
        bool microphoneOn = (microphoneButton.GetDimensionIndex() == 1);
        PhotonVoiceNetwork.Instance.PrimaryRecorder.TransmitEnabled = microphoneOn;
    }
}
