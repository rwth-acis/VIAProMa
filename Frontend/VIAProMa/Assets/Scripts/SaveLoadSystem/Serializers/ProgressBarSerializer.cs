using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProgressBarController))]
public class ProgressBarSerializer : MonoBehaviour, ISerializable
{
    private const string lengthKey = "prgBar_length";

    private ProgressBarController controller;

    private void Awake()
    {
        controller = GetComponent<ProgressBarController>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        float length = SerializedObject.TryGet(lengthKey, serializedObject.Integers, gameObject, out bool found);
        if (found)
        {
            controller.Length = length;
        }
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Floats.Add(lengthKey, controller.Length);
        return serializedObject;
    }
}
