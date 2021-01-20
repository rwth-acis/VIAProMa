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

        public List<Tuple<String, bool[]>> SerializeableSelection
        {
            get {
                return (selection.Where(t => t.Value != null).Select(t => new Tuple<String, bool[]>(Flags.HasFlag(PollOptions.Public)? t.Key.NickName : "Anonymous" , t.Value)).ToList());
            }
        }
        
        private Dictionary<Player, bool[]> selection;
        private byte remainingCount;
        private const int timeOutInSeconds = 1;  


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

            startPoll();
        }

        private void startPoll(){
            remainingCount = PollHandler.Instance.StartPoll(Question, Answers, End, Flags);
            PollHandler.Instance.PollAcknowledged += onAck;
            PollHandler.Instance.PollRespond += onResponse;
        }

        private void onAck(object sender, PollAcknowledgedEventArgs args){
            if(selection.ContainsKey(args.MessageSender) || remainingCount == 0){
                return;
            }
            remainingCount--;
            if(args.State){
                selection.Add(args.MessageSender, null);
            }
        }

        private void onResponse(object sender, PollRespondEventArgs args){
            if(!selection.ContainsKey(args.MessageSender)){
                return;
            }
            // bool[] argSelection = args.Selection;  
            // if(args.Selection.Length != Answers.Length){
            //     argSelection = new bool[Answers.Length];
            // }
            selection[args.MessageSender] =  args.Selection;

            if(selection.All(t => t.Value != null)){
                final();
            }
        }

        private void final(){
            // TODO
            // Visualize and stuff


            endTimer.Dispose();
            PollHandler.Instance.PollRespond -= onResponse;
            PollHandler.Instance.PollAcknowledged -= onAck;
        }

        public void endPoll(){
            PollHandler.Instance.EndPoll();
            endTimer = new System.Threading.Timer( x => {
                final();
            },null, TimeSpan.FromSeconds(timeOutInSeconds), Timeout.InfiniteTimeSpan);
        }


    }
}