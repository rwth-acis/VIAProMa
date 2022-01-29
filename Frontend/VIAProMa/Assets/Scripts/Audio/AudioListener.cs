using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.Visualizations.ProgressBars;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using i5.VIAProMa.UI.MainMenuCube;
using Microsoft.MixedReality.Toolkit.UI;

namespace i5.VIAProMa.Audio
{
    public class AudioListener : MonoBehaviourPunCallbacks
    {

        public override void OnConnected()
        {
            base.OnConnected();
            ChatManager.Instance.MessageReceived += OnMessageReceived;
            ProgressBarController.OnPercentageDoneChange += OnProgressbarDoneChanged;
            BuildingProgressBarVisuals.OnPercentageDoneChange += OnBuildingvisualDoneChanged;
            MainMenuActions.OnToggleMicrophone += OnToggleMic;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            ChatManager.Instance.MessageReceived -= OnMessageReceived;
            ProgressBarController.OnPercentageDoneChange -= OnProgressbarDoneChanged;
            BuildingProgressBarVisuals.OnPercentageDoneChange -= OnBuildingvisualDoneChanged;
            MainMenuActions.OnToggleMicrophone -= OnToggleMic;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            AudioManager.instance.PlayLoginSound(PhotonView.Find(newPlayer.ActorNumber).gameObject.transform.position);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            AudioManager.instance.PlayLogoffSound(PhotonView.Find(otherPlayer.ActorNumber).gameObject.transform.position);
        }

        /// <summary>
        /// Plays the clip for message receiving once, using a PlaySoundOnceAt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnMessageReceived(object sender, ChatMessageEventArgs args)
        {
            AudioManager.instance.PlayMessageSound();
        }

        /// <summary>
        /// Called when the progressbar changes its value for done percentage. Only plays sound if newValue > oldValue
        /// </summary>
        public void OnProgressbarDoneChanged(ProgressBarController bar, float oldValue, float newValue)
        {
            if (newValue <= oldValue) return;
            AudioManager.instance.PlayProgressBarSound(bar.transform.position);
        }

        /// <summary>
        /// Plays the Building Progessbar sound if there was progress
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public void OnBuildingvisualDoneChanged(BuildingProgressBarVisuals bar, float oldValue, float newValue)
        {
            if (newValue <= oldValue) return;
            AudioManager.instance.PlayBuildingProgressSound(bar.transform.position);
        }

        /// <summary>
        /// Decides which Microphone sound to play
        /// </summary>
        /// <param name="button"></param>
        /// <param name="micStatus"></param>
        public void OnToggleMic(Interactable button, bool micStatus)
        {
            Vector3 at = button.transform.position;
            if (micStatus) AudioManager.instance.PlayMicOnSound(at);
            else AudioManager.instance.PlayMicOffSound(at);
        }


    }
}