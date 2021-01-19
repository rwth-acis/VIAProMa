using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class PollDisplayEventArgs : EventArgs
    {
        public enum DisplayType {Bar, Cake};
        public short PollId {get; private set;}
        public DisplayType Display {get; private set;}

        public PollDisplayEventArgs(short pollId, DisplayType displayType){
            PollId = pollId;
            Display = displayType;
        }
    }
}
