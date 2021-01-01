using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Chat
{
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
}