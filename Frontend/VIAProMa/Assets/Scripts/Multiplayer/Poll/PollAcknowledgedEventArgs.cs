using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class PollAcknowledgedEventArgs : EventArgs
    {
        public Player MessageSender {get; private set;}
        public bool State {get; private set;}

        public PollAcknowledgedEventArgs(bool state, Player messageSender){
            MessageSender = messageSender;
            State = state;
        }
    }
}
