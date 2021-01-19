using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class PollRespondEventArgs : EventArgs
    {
        public bool[] Selection {get; private set;}
        public Player MessageSender {get; private set;}

        public PollRespondEventArgs(bool[] selection, Player messageSender){
            Selection = selection;
            MessageSender = messageSender;
        }
    }
}
