using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Serializes the data of the connecting lines
/// </summary>
[RequireComponent(typeof(LineDrawLogic))]
public class LinesSerializer : MonoBehaviour, ISerializable
{
    private const string lineNumberKey = "lineNumber";

    private LineDrawLogic lineDrawLogic;

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        lineDrawLogic = GetComponent<LineDrawLogic>();
    }

    /// <summary>
    /// Applies the settings which are stored in the serializedObject to the connecting lines
    /// </summary>
    /// <param name="serializedObject">The serialized object with the save data</param>
    public void Deserialize(SerializedObject serializedObject)
    {
        int lineIndex = SerializedObject.TryGet(lineNumberKey, serializedObject.Integers, gameObject, out bool found);
        if (found)
        {
            lineDrawLogic.LineIndex = lineIndex;
        }
    }

    /// <summary>
    /// Writes the data of the progress bar into a save data object
    /// </summary>
    /// <returns>The saved data of the building progress bar component</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(lineNumberKey, lineDrawLogic.LineIndex);
        return serializedObject;
    }
}
