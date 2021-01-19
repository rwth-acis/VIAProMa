using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace i5.VIAProMa.Multiplayer.Poll{
    public class Poll
    {
        public string Question {get; private set;}
        public string[] Answers {get; private set;}
        
        public PollOptions Flags {get; private set;}
        public DateTime End {get; private set;}

        public List<Tuple<int, bool[]>> Selection
        {
            get {
                return (selection.Select(t => new Tuple<int, bool[]>(Flags.HasFlag(PollOptions.Public)? t.Key.ActorNumber : -1, t.Value)).ToList());
            }
        }
        
        private Dictionary<Player, bool[]> selection;
        private byte remainingCount;

        private Timer endTimer;

        public Poll(string question, string[] answers, PollOptions flags, DateTime end){
            Question = question;
            Answers = answers;
            Flags = flags;
            End = end;
            selection = new Dictionary<Player, bool[]>();

            TimeSpan timeToGo = end - DateTime.Now;
            if(timeToGo > TimeSpan.Zero){
                endTimer = new System.Threading.Timer( x => {
                    this.endPoll();
                },null,timeToGo,Timeout.InfiniteTimeSpan);
            }else{
                //TODO assert flag
            }

            Task.Run(startPoll);
        }

        private async void startPoll(){
            remainingCount = await PollHandler.Instance.StartPoll(Question, Answers, End, Flags);
            PollHandler.Instance.PollRespond += onResponse;
        }

        private void onResponse(object sender, PollRespondEventArgs args){
            if(remainingCount == 0){
                return;
            }
            remainingCount--;
            if(selection.ContainsKey(args.MessageSender)){
                return;
            }
            bool[] argSelection = args.Selection;  
            if(args.Selection.Length != Answers.Length){
                argSelection = new bool[Answers.Length];
            }
            selection.Add(args.MessageSender, args.Selection);

            if(remainingCount == 0){ //Bug: Person joing after poll start but leaving before poll end will decrement the remaining count, resulting in one poll participant beign ignored 
                final();
            }
        }

        private void final(){
            // TODO
            // Visualize and stuff


            endTimer.Dispose();
            PollHandler.Instance.PollRespond -= onResponse;
        }

        public void endPoll(){
            PollHandler.Instance.EndPoll();
        }


    }
}