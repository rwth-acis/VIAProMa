using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;

namespace i5.VIAProMa.Multiplayer.Poll
{
    /// <summary>
	/// Boolean options for Polls
	/// </summary>
    [Flags]
    public enum PollOptions : byte
    {
        None = 0,
        Public = 1,
        MultipleChoice = 2,
        Countdown = 4,
        SaveResults = 8,
        RealtimeViz = 16
    }

    /// <summary>
	/// A stored completed poll or a frozen state of a poll
	/// </summary>
    [Serializable]
    public class SerializablePoll
    {
        public string Question;
        public string[] Answers;
        public PollOptions Flags;
        public List<Tuple<string,bool[]>> SerializeableSelection;
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
            for (int i = 0; i < answersLength; i++)
            {
                byte[] answerBytes = new byte[data[offset]];
                Array.Copy(data, offset + 1, answerBytes, 0, data[offset]);
                answers[i] = new string(System.Text.Encoding.UTF8.GetChars(answerBytes));
                offset += 1 + data[offset];
            }
            result.Answers = answers;
            result.Flags = (PollOptions)data[offset]; //needs to be changed if PollOptions gets too big
            offset++;
            var selectionLength = data[offset];
            offset++;
            result.SerializeableSelection = new List<Tuple<string,bool[]>>();
            for (int i = 0; i < selectionLength; i++)
            {
                byte[] nameBytes = new byte[data[offset]];
                Array.Copy(data, offset + 1, nameBytes, 0, data[offset]);
                offset += 1 + data[offset];
                byte[] selBytes = new byte[data[offset]];
                Array.Copy(data, offset + 1, selBytes, 0, data[offset]);
                result.SerializeableSelection.Add(new Tuple<string,bool[]> (new string(System.Text.Encoding.UTF8.GetChars(nameBytes)), selBytes.Select(b => Convert.ToBoolean(b)).ToArray()));
                offset += 1 + data[offset];
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
            var p = (SerializablePoll)serializablePoll;
            List<byte> data = new List<byte>();
            byte[] questionBytes = System.Text.Encoding.UTF8.GetBytes(p.Question);
            // Store length and content of question
            data.Add((byte)questionBytes.Length);
            data.AddRange(questionBytes);
            // Store number of answers
            data.Add((byte)p.Answers.Length);
            foreach (String a in p.Answers)
            {
                // Store length and content of answer
                byte[] answerBytes = System.Text.Encoding.UTF8.GetBytes(a);
                data.Add((byte)answerBytes.Length);
                data.AddRange(answerBytes);
            }
            // Store flags
            data.Add((byte)p.Flags); //one byte should be enough for now
            // Store number of responses
            data.Add((byte)p.SerializeableSelection.Count);
            for (int i = 0; i < p.SerializeableSelection.Count; i++)
            {
                var tuple = p.SerializeableSelection[i];
                // Store length and content of name
                byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(tuple.Item1);
                data.Add((byte)nameBytes.Length);
                data.AddRange(nameBytes);
                // store length and content of selection
                data.Add((byte)(tuple.Item2.Length));
                data.AddRange(tuple.Item2.Select(b => Convert.ToByte(b)));
            }
            return data.ToArray();
        }

        public static SerializablePoll FromPoll(Poll poll)
        {
            var result = new SerializablePoll();
            result.Answers = poll.Answers;
            result.Flags = poll.Flags;
            result.Question = poll.Question;
            result.SerializeableSelection = poll.Selection;
            return result;
        }
    }

    /// <summary>
	/// A representation of an active poll, with logic and current state
	/// </summary>
    public class Poll
    {
        private Dictionary<Player, bool[]> selection;

        public string Question { get; private set; }
        public string[] Answers { get; private set; }
        public PollOptions Flags { get; private set; }
        public bool IsEnded { get; set; }
        public bool IsFinalized { get; set; }
        public int SyncedEndTime { get; set; }

        /// <summary>
        /// Current poll responses as it should be saved in accordance with Poll Options
        /// </summary>
        public List<Tuple<string, bool[]>> Selection
        {
            get
            {
                return selection.Where(t => t.Value != null).Select(t => new Tuple<string,bool[]> (Flags.HasFlag(PollOptions.Public) ? t.Key.NickName : "Anonymous", t.Value )).ToList();
            }
        }

        /// <summary>
        /// Number of participants currently registered in the poll
        /// </summary>
        public int ParticipantCount
        {
            get
            {
                return selection.Count();
            }
        }

        /// <summary>
        /// Number of responses currently logged in the poll
        /// </summary>
        public int ResponseCount
        {
            get
            {
                return selection.Where(t => t.Value != null).Count();
            }
        }

        /// <summary>
        /// Creates a new Poll with the given setup
        /// </summary>
        /// <param name="question">Question text</param>
        /// <param name="answers">Array of answer options</param>
        /// <param name="flags">Boolean options for this poll</param>
        public Poll(string question, string[] answers, PollOptions flags, int synedEndTime)
        {
            Question = question;
            Answers = answers;
            Flags = flags;
            SyncedEndTime = synedEndTime;
            selection = new Dictionary<Player, bool[]>();
            IsEnded = false;
            IsFinalized = false;
        }

        /// <summary>
        /// Updates the participation status of the given player
        /// </summary>
        /// <param name="sender">The player to update the status of</param>
        /// <param name="state">If true, player participates in poll, else he is unregistered</param>
        /// <returns>True if all registered poll participants have send a response, false if more are expected</returns>
         public bool OnStatus(Player sender, bool state)
        {
            if (selection.ContainsKey(sender))
            {
                if (!state)
                {
                    selection.Remove(sender);
                    return selection.All(t => t.Value != null);
                }
                return false;
            }
            else
            {
                if (state)
                {
                    selection.Add(sender, null);
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
                return false;
            }
            selection[sender] = answers;
            return selection.All(t => t.Value != null);
        }
    }
}