using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSerialiser : MonoBehaviour, ISerializable
{
    ConnectionCurve curve;
    LineController controller;

    private void Awake()
    {
        curve = GetComponent<ConnectionCurve>();
        controller = GameObject.Find("LineController(Clone)").GetComponent<LineController>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        if (serializedObject.Strings.TryGetValue("start", out string startID) && serializedObject.Strings.TryGetValue("goal", out string goalID))
        {
            var serializers = FindObjectsOfType<Serializer>();
            //foreach (var serializer in serializers)
            //{
            //    if (serializer.Id == startID)
            //    {
            //        curve.start = serializer.gameObject;
            //    }
            //    if (serializer.Id == goalID)
            //    {
            //        curve.goal = serializer.gameObject;
            //    }
            //}
            var test = SaveLoadManager.Instance.GetSerializer(startID);
            //curve.start = SaveLoadManager.Instance.GetSerializer(startID).gameObject;
            //curve.goal = SaveLoadManager.Instance.GetSerializer(goalID).gameObject;
            curve.startID = startID;
            curve.goalID = goalID;
        }
    }

    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        string startID = curve.start.GetComponent<Serializer>().Id;
        string goalID = curve.goal.GetComponent<Serializer>().Id;
        serializedObject.Strings.Add("start",startID);
        serializedObject.Strings.Add("goal",goalID);
        return serializedObject;
    }
}
