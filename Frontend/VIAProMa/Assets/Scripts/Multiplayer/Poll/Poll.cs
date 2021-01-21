using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;
using System.Timers;

namespace i5.VIAProMa.Multiplayer.Poll
{
    [Flags]
    public enum PollOptions
    {
        None = 0,
        Public = 1,
        MultipleChoice = 2,
        Countdown = 4
    }

    public class Poll
    {
        private Dictionary<Player, bool[]> selection;

        public string Question { get; private set; }
        public string[] Answers { get; private set; }
        public PollOptions Flags { get; private set; }
        
        public List<Tuple<String, bool[]>> SerializeableSelection
        {
            get
            {
                return (selection.Select(t => new Tuple<String, bool[]>(Flags.HasFlag(PollOptions.Public)? t.Key.NickName : "Anonymous" , t.Value)).ToList());
            }
        }
        public int[] AccumulatedResults
        {
            get
            {
                List<Tuple<String, bool[]>> sel = SerializeableSelection;
                int[] results = new int[Answers.Length];
                for (int i = 0; i < Answers.Length; i++)
                {
                    for (int j = 0; j < sel.Count; j++)
                    {
                        results[i] += sel[j].Item2[i]? 1 : 0;
                    }
                }
                return results;
            }
        }

        public Poll(string question, string[] answers, PollOptions flags)
        {
            Question = question;
            Answers = answers;
            Flags = flags;
            selection = new Dictionary<Player, bool[]>();
        }

        public bool OnStatus(Player sender, bool state)
        {
            if (selection.ContainsKey(sender))
            {
                if (!state)
                {
                    Debug.Log("Received nak during poll from " + sender.NickName + "! Unregistering!");
                    selection.Remove(sender);
                    return selection.All(t => t.Value != null);
                }
                Debug.LogWarning("Received double ack from " + sender.NickName + "! Should not happen!");
                return false;
            }
            else
            {
                if (state)
                {
                    selection.Add(sender, null);
                    Debug.Log("Player " + sender.NickName + " participates in poll!");
                }
                else
                {
                    Debug.LogWarning("Received nak from unregistered " + sender.NickName + "! Should not happen!");
                }
                return false;
            }
        }

        public bool OnResponse(Player sender, bool[] answers)
        {
            if (!selection.ContainsKey(sender))
            {
                Debug.Log("Received response from unregistered player " + sender.NickName + "! Should not happen!");
                return false;
            }
            selection[sender] = answers;
            Debug.Log("Logged response from " + sender.NickName + "!");
            return selection.All(t => t.Value != null);
        }
    }
}