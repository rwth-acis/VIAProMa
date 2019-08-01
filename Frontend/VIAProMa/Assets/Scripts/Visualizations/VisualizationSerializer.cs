using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Visualization))]
public class VisualizationSerializer : MonoBehaviour, ISerializable
{
    private const string titleKey = "visualizationTitle";

    private Visualization visualization;

    public void Deserialize(SerializedObject serializedObject)
    {
        visualization.Title = serializedObject.Strings[titleKey];
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(titleKey, visualization.Title);
        return serializedObject;
    }
}
