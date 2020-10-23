using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSerialiser : MonoBehaviour, ISerializable
{
    ConnectionCurve curve;

    private void Awake()
    {
        curve = GetComponent<ConnectionCurve>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        curve.startID = serializedObject.Strings["startID"];
        curve.goalID = serializedObject.Strings["goalID"];
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        string startID = curve.start.GetComponent<Serializer>().Id;
        string goalID = curve.goal.GetComponent<Serializer>().Id;
        serializedObject.Strings.Add("startID", startID);
        serializedObject.Strings.Add("goalID",goalID);
        return serializedObject;
    }
}
