﻿using i5.VIAProMa.SaveLoadSystem.Core;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.Multiplayer.Poll;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    public class PollSerializer : MonoBehaviour, ISerializable 
    {
        private const string pollKey = "poll";
        private const string countKey = "pollCount";
        
        private static string ConstructKey(string key, int index)
        {
            return key + "#" + index;
        }

        public void Deserialize(SerializedObject serializedObject)
        {
            List<SerializablePoll> polls = new List<SerializablePoll>();

            var count = SerializedObject.TryGet(countKey, serializedObject.Integers, gameObject, out bool found);
            if(found){
                for(int i = 0; i<count; i++){
                    byte[] serializedPoll = SerializedObject.GetList(ConstructKey(pollKey, i),serializedObject.Integers).Select(v => (byte)v).ToArray();
                    polls.Add((SerializablePoll)SerializablePoll.Deserialize(serializedPoll));
                }
                PollHandler.Instance.savedPolls = polls;
            }
        }

        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            List<byte[]> serializedPolls = PollHandler.Instance.savedPolls.Select(p => SerializablePoll.Serialize(p)).ToList();
            serializedObject.Integers[countKey] = serializedPolls.Count;
            for(int i = 0; i<serializedPolls.Count; i++){
                SerializedObject.AddList(ConstructKey(pollKey, i), serializedPolls[i].Select(b => (int) b).ToList(),serializedObject.Integers); //saving bytes into ints...
            }
            return serializedObject;
        }
    }

}