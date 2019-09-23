using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializes the data of the building progress bar
/// </summary>
[RequireComponent(typeof(BuildingProgressBarVisuals))]
public class BuildingProgressBarSerializer : MonoBehaviour, ISerializable
{
    private const string buildingNumberKey = "buildingNumber";

    private BuildingProgressBarVisuals buildingProgressBarVisuals;

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        buildingProgressBarVisuals = GetComponent<BuildingProgressBarVisuals>();
    }

    /// <summary>
    /// Applies the settings which are stored in the serializedObject to the building progress bar
    /// </summary>
    /// <param name="serializedObject">The serialized object with the save data</param>
    public void Deserialize(SerializedObject serializedObject)
    {
        int buildingModelIndex = SerializedObject.TryGet(buildingNumberKey, serializedObject.Integers, gameObject, out bool found);
        if (found)
        {
            buildingProgressBarVisuals.BuildingModelIndex = buildingModelIndex;
        }
    }

    /// <summary>
    /// Writes the data of the progress bar into a save data object
    /// </summary>
    /// <returns>The saved data of the building progress bar component</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(buildingNumberKey, buildingProgressBarVisuals.BuildingModelIndex);
        return serializedObject;
    }
}
