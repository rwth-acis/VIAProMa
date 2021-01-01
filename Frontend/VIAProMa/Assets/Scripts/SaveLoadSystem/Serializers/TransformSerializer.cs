using i5.VIAProMa.SaveLoadSystem.Core;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Serializers
{
    /// <summary>
    /// Serialzes and deserializes transform data such as the position, rotation and scale of an object
    /// </summary>
    public class TransformSerializer : MonoBehaviour, ISerializable
    {
        private const string positionKey = "position";
        private const string rotationKey = "rotation";
        private const string scaleKey = "scale";

        /// <summary>
        /// Deserializes a serialized object and applies the fetched data to the transform of this component
        /// Expects an entry for position, rotation and scale in the serializedObject
        /// </summary>
        /// <param name="serializedObject">The object which contains the serialized data</param>
        public void Deserialize(SerializedObject serializedObject)
        {
            transform.localPosition = serializedObject.Vector3s[positionKey];
            transform.localRotation = serializedObject.Quaternions[rotationKey];
            transform.localScale = serializedObject.Vector3s[scaleKey];
        }

        /// <summary>
        /// Serializes the transform data of the position, rotation and scale to a serialized object
        /// </summary>
        /// <returns>The serialized object with the position, rotation and scale entries</returns>
        public SerializedObject Serialize()
        {
            SerializedObject serializedObject = new SerializedObject();
            serializedObject.Vector3s.Add(positionKey, transform.localPosition);
            serializedObject.Quaternions.Add(rotationKey, transform.localRotation);
            serializedObject.Vector3s.Add(scaleKey, transform.localScale);
            return serializedObject;
        }
    }
}