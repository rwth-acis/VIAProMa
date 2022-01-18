using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.Visualizations.ProgressBars;
using i5.VIAProMa.Visualizations.BuildingProgressBar;


public class AudioListener : MonoBehaviourPunCallbacks
{

    public override void OnConnected()
    {
        base.OnConnected();
        ChatManager.Instance.MessageReceived += OnMessageReceived;
        ProgressBarController.OnPercentageDoneChange += OnProgressbarDoneChanged;
        BuildingProgressBarVisuals.OnPercentageDoneChange += OnBuildingvisualDoneChanged;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        ChatManager.Instance.MessageReceived -= OnMessageReceived;
        ProgressBarController.OnPercentageDoneChange-= OnProgressbarDoneChanged;
        BuildingProgressBarVisuals.OnPercentageDoneChange -= OnBuildingvisualDoneChanged;
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

    public void OnBuildingvisualDoneChanged(BuildingProgressBarVisuals bar, float oldValue, float newValue)
    {
        if (newValue <= oldValue) return;
        AudioManager.instance.PlayBuildingProgressSound(bar.transform.position);
    }

}
