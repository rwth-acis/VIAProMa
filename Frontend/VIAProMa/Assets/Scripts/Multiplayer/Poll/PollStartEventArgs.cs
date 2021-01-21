using Photon.Realtime;
using System;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class PollStartEventArgs : EventArgs
    {
        public string Question { get; private set; }
        public string[] Answers { get; private set; }
        public Player MessageSender { get; private set; }
        public DateTime End { get; private set; }
        public PollOptions Flags { get; private set; }

        public PollStartEventArgs(string question, string[] answers, DateTime end, Player messageSender, PollOptions flags)
        {
            Question = question;
            Answers = answers;
            MessageSender = messageSender;
            End = end;
            Flags = flags;
        }
    }
}
