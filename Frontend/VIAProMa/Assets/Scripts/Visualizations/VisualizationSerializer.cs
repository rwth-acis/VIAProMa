using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializes and deserialized a visualization and its content
/// </summary>
[RequireComponent(typeof(Visualization))]
public class VisualizationSerializer : MonoBehaviour, ISerializable
{
    /// <summary>
    /// Key to store and retrieve the title of the visualization
    /// </summary>
    private const string titleKey = "visualizationTitle";
    /// <summary>
    /// Key to store and retrieve the content of the visualization
    /// </summary>
    private const string contentKey = "content";

    /// <summary>
    /// The visualization to serialize/deserialize
    /// </summary>
    private Visualization visualization;

    /// <summary>
    /// Deserializes the given serializedObject and applies the found relevant properties to the visualization
    /// </summary>
    /// <param name="serializedObject">A serialized object which contains the properties for the visualization</param>
    public void Deserialize(SerializedObject serializedObject)
    {
        visualization.Title = serializedObject.Strings[titleKey];
    }

    /// <summary>
    /// Serializes the visualization into a SerializedObject
    /// </summary>
    /// <returns>A SerializedObject which contains the relevant properties of the visualization</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(titleKey, visualization.Title);
        return serializedObject;
    }
}
