using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HeatmapDataManagement))]
public class HeatmapSerializer : MonoBehaviour, ISerializable
{
    private const string dataKey = "heatmap_data";

    private HeatmapDataManagement dataManagement;

    private void Awake()
    {
        dataManagement = GetComponent<HeatmapDataManagement>();
    }

    public void Deserialize(SerializedObject serializedObject)
    {
        string stringData = SerializedObject.TryGet(dataKey, serializedObject.Strings, gameObject, out bool found);
        if (found)
        {
            dataManagement.data = HeatmapDataManagement.StringToArray(stringData);
        }
    }

    public SerializedObject Serialize()
    {
        string stringdata = HeatmapDataManagement.ArrayToString(dataManagement.data);
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(dataKey, stringdata);
        return serializedObject;
    }
}
