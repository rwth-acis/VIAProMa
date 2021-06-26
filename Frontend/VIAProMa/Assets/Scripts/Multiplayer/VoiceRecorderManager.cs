using Photon.Voice.PUN;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
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
           //PhotonVoiceNetwork.Instance.PrimaryRecorder.Init(PhotonVoiceNetwork.Instance.VoiceClient);
            Debug.Log("Voice Recorder initialized");
        }

    }
}