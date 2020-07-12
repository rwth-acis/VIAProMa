using HoloToolkit.Unity;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class ChatManager : Singleton<ChatManager>
{
    public string setUsername;

    private PhotonView photonView;

    public event EventHandler<ChatMessageEventArgs> MessageReceived;

    public bool RecordMessages { get; set; } = true;

    public List<ChatMessageEventArgs> ChatMessages { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
        ChatMessages = new List<ChatMessageEventArgs>();
    }

    public async void SendChatMessage(string text)
    {
        short textId = await NetworkedStringManager.StringToId(text);
        photonView.RPC("ChatMessageReceived", RpcTarget.All, textId) ;

    }

    [PunRPC]
    private async void ChatMessageReceived(short textId, PhotonMessageInfo messageInfo)
    {
        string text = await NetworkedStringManager.GetString(textId);
        ChatMessageEventArgs args = new ChatMessageEventArgs(text, messageInfo.Sender);
        if (RecordMessages)
        {
            ChatMessages.Add(args);
        }
        MessageReceived?.Invoke(this, args);
    }

    public void AddLocalMessage(string text)
    {
        ChatMessageEventArgs args = new ChatMessageEventArgs(text, null);
        if (RecordMessages)
        {
            ChatMessages.Add(args);
        }
        MessageReceived?.Invoke(this, args);
    }

    public void sendTextTo(string username, string text)
    {
        SendChatMessage(username + "@ " + text);
    }

    public void getUsername(string username)
    {
        string setUsername = username;
    }

}

