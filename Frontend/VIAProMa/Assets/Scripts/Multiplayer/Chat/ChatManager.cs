using HoloToolkit.Unity;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ChatManager : Singleton<ChatManager>
{
    private PhotonView photonView;

    public event EventHandler<ChatMessageEventArgs> MessageReceived;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }

    public async void SendChatMessage(string text)
    {
        short textId = await NetworkedStringManager.StringToId(text);
        photonView.RPC("ChatMessageReceived", RpcTarget.All, textId);
    }

    [PunRPC]
    private async void ChatMessageReceived(short textId, PhotonMessageInfo messageInfo)
    {
        string text = await NetworkedStringManager.GetString(textId);
        MessageReceived?.Invoke(this, new ChatMessageEventArgs(text, messageInfo.Sender));
    }
}
