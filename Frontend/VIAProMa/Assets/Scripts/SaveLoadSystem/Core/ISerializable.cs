using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
