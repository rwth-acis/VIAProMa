using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class PollEndEventArgs : EventArgs
    {
        public Player MessageSender {get; private set;}

        public PollEndEventArgs(Player messageSender){
            MessageSender = messageSender;
        }
    }
}
