using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll
{
    public class PollDisplayEventArgs : EventArgs
    {
        public enum DisplayType { Bar, Cake };
//        public short PollId {get; private set;}
        public int[] PollResults { get; private set; }
        public DisplayType Display { get; private set; }

//        public PollDisplayEventArgs(short pollId, DisplayType displayType)
        public PollDisplayEventArgs(int[] results, DisplayType displayType)
        {
//            PollId = pollId;
            PollResults = results;
            Display = displayType;
        }
    }
}
