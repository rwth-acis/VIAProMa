using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessageEventArgs : EventArgs
{
    public string Message { get; private set; }
    public Player MessageSender { get; private set; }

    public ChatMessageEventArgs(string message, Player messageSender)
    {
        Message = message;
        MessageSender = messageSender;
    }
}
