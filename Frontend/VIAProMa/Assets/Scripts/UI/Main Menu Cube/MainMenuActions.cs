using Microsoft.MixedReality.Toolkit.UI;
using Photon.Voice.PUN;
using UnityEngine;

namespace i5.VIAProMa.UI.MainMenuCube
{
    public class MainMenuActions : MonoBehaviour
    {
        [SerializeField] private Interactable microphoneButton;

        public void ToggleMicrophone()
        {
            bool microphoneOn = (microphoneButton.CurrentDimension == 1);
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = microphoneOn;
        }
    }
}