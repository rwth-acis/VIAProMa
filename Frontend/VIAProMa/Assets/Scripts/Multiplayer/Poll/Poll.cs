using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;

namespace i5.VIAProMa.Multiplayer.Poll
{
    [Flags]
    public enum PollOptions : byte
    {
        None = 0,
        Public = 1,
        MultipleChoice = 2,
        Countdown = 4,
        SaveResults = 8
    }

    [Serializable]
    public struct SelectionResult
    {
        public string Item1;
        public bool[] Item2;
    }

    [Serializable]
    public class SerializablePoll
    {
        public string Question;
        public string[] Answers;
        public PollOptions Flags;
        public List<SelectionResult> SerializeableSelection;
        public static byte SerializeablePollCode = 255;
        public int[] AccumulatedResult
        {
            get 
            {
                int[] results = new int[Answers.Length];
                for (int i = 0; i < Answers.Length; i++)
                {
                    for (int j = 0; j < SerializeableSelection.Count; j++)
                    {
                        results[i] += SerializeableSelection[j].Item2[i]? 1 : 0;
                    }
                }
                return results;
            }
        }
        /// <summary>
        /// Deserializes a Poll from a byte[]
        /// Assumes correctly formated input
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] data)
        {
            var result = new SerializablePoll();
            int offset = 0;
            byte[] questionBytes = new byte[data[0]];
            Array.Copy(data, 1, questionBytes, 0, data[0]);
            result.Question = new string(System.Text.Encoding.UTF8.GetChars(questionBytes));
            offset += 1 + data[0];
            int answersLength = data[offset];
            string[] answers = new string[answersLength];
            offset++;
            for(int i = 0; i<answersLength; i++)
            {
                byte[] answerBytes = new byte[data[offset]];
                Array.Copy(data, offset+1, answerBytes, 0, data[offset]);
                answers[i] = new string(System.Text.Encoding.UTF8.GetChars(answerBytes));
                offset += 1+data[offset];
            }
            result.Answers = answers;
            result.Flags = (PollOptions) data[offset]; //needs to be changed if PollOptions gets too big
            offset++;
            var selectionLength = data[offset];
            offset++;
            result.SerializeableSelection = new List<SelectionResult>();
            for(int i = 0; i<selectionLength; i++)
            {
                byte[] nameBytes = new byte[data[offset]];
                Array.Copy(data, offset+1, nameBytes, 0, data[offset]);
                offset += 1+data[offset];
                byte[] selBytes = new byte[data[offset]];
                Array.Copy(data, offset+1, selBytes, 0, data[offset]);
                result.SerializeableSelection.Add(new SelectionResult { Item1 = new string(System.Text.Encoding.UTF8.GetChars(nameBytes)), Item2 = selBytes.Select(b => Convert.ToBoolean(b)).ToArray() });
                offset += 1+data[offset];
            }
            return result;
        }
        /// <summary>
        /// Serializes a poll into a byte[] for sending via photon
        /// </summary>
        /// <param name="serializablePoll"></param>
        /// <returns></returns>
        public static byte[] Serialize(object serializablePoll)
        {
            var p = (SerializablePoll) serializablePoll;
            List<byte> data = new List<byte>();
            byte[] questionBytes = System.Text.Encoding.UTF8.GetBytes(p.Question);
            data.Add((byte)questionBytes.Length);
            data.AddRange(questionBytes);
            data.Add((byte)p.Answers.Length);
            foreach(String a in p.Answers)
            {
                byte[] answerBytes = System.Text.Encoding.UTF8.GetBytes(a);
                data.Add((byte)answerBytes.Length);
                data.AddRange(answerBytes);
            }
            data.Add((byte)p.Flags); //one byte should be enough for now
            data.Add((byte)p.SerializeableSelection.Count);
            for(int i = 0; i < p.SerializeableSelection.Count; i++)
            {
                var tuple = p.SerializeableSelection[i];
                byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(tuple.Item1);
                data.Add((byte)nameBytes.Length);
                data.AddRange(nameBytes);
                data.Add((byte)tuple.Item2.Length);
                data.AddRange(tuple.Item2.Select(b => Convert.ToByte(b)));
            }
            return data.ToArray();
        }
        
        public static SerializablePoll FromPoll(Poll poll){
            var result = new SerializablePoll();
            result.Answers = poll.Answers;
            result.Flags = poll.Flags;
            result.Question = poll.Question;
            result.SerializeableSelection = poll.SerializeableSelection.Select(t => new SelectionResult {Item1 = t.Item1, Item2 = t.Item2}).ToList();
            return result;
        }
    }

    /**
     * Data and basic logic for Polls
     */
    public class Poll
    {
        private Dictionary<Player, bool[]> selection;

        public string Question { get; private set; }
        public string[] Answers { get; private set; }
        public PollOptions Flags { get; private set; }
        public bool IsEnded { get; set; }
        public bool IsFinalized { get; set; }
        
        /**
         * Poll results for each user as it should be saved in accordance with Poll Options
         */
        public List<Tuple<String, bool[]>> SerializeableSelection
        {
            get
            {
                return (selection.Select(t => new Tuple<String, bool[]>(Flags.HasFlag(PollOptions.Public)? t.Key.NickName : "Anonymous" , t.Value)).ToList());
            }
        }
        
        /**
         * Accumulated Poll results for each answer
         */
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
            IsEnded = false;
            IsFinalized = false;
        }

        /**
         * Update player participation status
         */
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

        /**
         * Set player poll response
         */
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