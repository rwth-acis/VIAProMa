using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSerializer : MonoBehaviour, ISerializer
{
    private const string positionKey = "position";
    private const string rotationKey = "rotation";
    private const string scaleKey = "scale";

    public void Deserialize(SerializedData serializedData)
    {
        transform.localPosition = serializedData.Vector3s[positionKey];
        transform.localRotation = serializedData.Quaternions[rotationKey];
        transform.localScale = serializedData.Vector3s[scaleKey];
    }

    public SerializedData Serialize()
    {
        SerializedData serializedData = new SerializedData();
        serializedData.Vector3s.Add(positionKey, transform.localPosition);
        serializedData.Quaternions.Add(rotationKey, transform.localRotation);
        serializedData.Vector3s.Add(scaleKey, transform.localScale);
        return serializedData;
    }
}
