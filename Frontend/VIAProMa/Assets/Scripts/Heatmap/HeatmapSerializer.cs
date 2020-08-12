using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Serializer to save and load heatmap between sessions
/// </summary>
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
            dataManagement.data = StringToArray(stringData);
        }
    }

    public SerializedObject Serialize()
    {
        string stringdata = ArrayToString(dataManagement.data);
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(dataKey, stringdata);
        return serializedObject;
    }

    public static int[,] StringToArray(string arrayString)
    {
        string[] lines = arrayString.Split('#');
        int[,] array = new int[lines.Length, lines.Length];
        for (int y = 0; y < array.GetLength(1); y++)
        {
            string[] sNums = lines[y].Split(';');
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int.TryParse(sNums[x], out array[x, y]);
            }
        }
        return array;
    }

    public static string ArrayToString(int[,] array)
    {
        string arrayString = "";
        for (int y = 0; y < array.GetLength(1); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                arrayString += array[x, y];
                if (x < array.GetLength(0) - 1)
                {
                    arrayString += ";";
                }
            }
            if (y < array.GetLength(1) - 1)
            {
                arrayString += "#";
            }
        }
        return arrayString;
    }
}
