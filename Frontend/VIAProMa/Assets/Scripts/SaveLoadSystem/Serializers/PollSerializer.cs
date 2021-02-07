using i5.VIAProMa.SaveLoadSystem.Core;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.Multiplayer.Poll;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    /// <summary>
    /// Serializes and Deserializes PollHandler.Instance.savedPolls to save on the Backend
    /// </summary>
    /// <remarks>
    /// This is a dirty hack and should not be looked at too closely   
    /// </remarks>
    public class PollSerializer : MonoBehaviour, ISerializable 
    {
        private const string pollKey = "poll";
        private const string countKey = "pollCount";
        
        private static string ConstructKey(string key, int index)
        {
            return key + "#" + index;
        }

        /// <summary>
        /// Deserialize the serialized poll data into the polls stored in this project
        /// </summary>
        /// <param name="serializedObject">The serialized poll data from the project save.</param>
        public void Deserialize(SerializedObject serializedObject)
        {
            List<SerializablePoll> polls = new List<SerializablePoll>();

            var count = SerializedObject.TryGet(countKey, serializedObject.Integers, gameObject, out bool found);
            if (found)
            {
                for (int i = 0; i < count; i++)
                {
                    byte[] serializedPoll = SerializedObject.GetList(ConstructKey(pollKey, i),serializedObject.Integers).Select(v => (byte)v).ToArray();
                    polls.Add((SerializablePoll)SerializablePoll.Deserialize(serializedPoll));
                }
                PollHandler.Instance.savedPolls = polls;
                PollHandler.Instance.UpdatePolls();
            }
        }

        /// <summary>
        /// Serializes the polls currently stored in the scene into the serialized poll data
        /// </summary>
        /// <returns>Serialized poll data to be stored with the project</returns>
        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            List<byte[]> serializedPolls = PollHandler.Instance.savedPolls.Select(p => SerializablePoll.Serialize(p)).ToList();
            serializedObject.Integers[countKey] = serializedPolls.Count;
            for (int i = 0; i < serializedPolls.Count; i++)
            {
                SerializedObject.AddList(ConstructKey(pollKey, i), serializedPolls[i].Select(b => (int)b).ToList(),serializedObject.Integers); //saving bytes into ints...
            }
            serializedObject.Bools["ROOM"] = true; // handle this as a Room Object
            return serializedObject;
        }
    }
}