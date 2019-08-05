using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingProgressBarVisuals))]
public class BuildingProgressBarSerializer : MonoBehaviour, ISerializable
{
    private const string buildingNumberKey = "buildingNumber";

    private BuildingProgressBarVisuals buildingProgressBarVisuals;

    private void Awake()
    {
        buildingProgressBarVisuals = GetComponent<BuildingProgressBarVisuals>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        buildingProgressBarVisuals.BuildingModelIndex = serializedObject.Integers[buildingNumberKey];
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Integers.Add(buildingNumberKey, buildingProgressBarVisuals.BuildingModelIndex);
        return serializedObject;
    }
}
