using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using i5.VIAProMa.Multiplayer.Chat;

public class AudioListener : MonoBehaviourPunCallbacks
{

    public void Start()
    {
        ChatManager.Instance.MessageReceived += OnMessageReceived;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.MessageReceived -= OnMessageReceived;
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
}
