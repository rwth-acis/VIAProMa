using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformSerializer : MonoBehaviour, ISerializable
{
    private const string positionKey = "position";
    private const string rotationKey = "rotation";
    private const string scaleKey = "scale";

    public void Deserialize(SerializedObject serializedObject)
    {
        transform.localPosition = serializedObject.Vector3s[positionKey];
        transform.localRotation = serializedObject.Quaternions[rotationKey];
        transform.localScale = serializedObject.Vector3s[scaleKey];
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Vector3s.Add(positionKey, transform.localPosition);
        serializedObject.Quaternions.Add(rotationKey, transform.localRotation);
        serializedObject.Vector3s.Add(scaleKey, transform.localScale);
        return serializedObject;
    }
}
