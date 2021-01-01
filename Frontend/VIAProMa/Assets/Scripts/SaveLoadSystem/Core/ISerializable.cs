namespace i5.VIAProMa.SaveLoadSystem.Core
{
    /// <summary>
    /// Interface for serializable content
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serializes data into a SerializedObject
        /// </summary>
        /// <returns>The SerializedObject with the packaged key value pairs</returns>
        SerializedObject Serialize();

        /// <summary>
        /// Deserializes a given SerializedObject and applies the values
        /// </summary>
        /// <param name="serializedObject">The SerializedObject which should be applied to the object</param>
        void Deserialize(SerializedObject serializedObject);

    }
}