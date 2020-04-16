using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializes and deserializes connection line data
/// </summary>
[RequireComponent(typeof(UpdatePosition))]
public class ConnectionLineSerializer : MonoBehaviour, ISerializable
{
    private const string startObjectPositionKey = "start";
    private const string destinationObjectPositionKey = "destination";
    private const string connectionLineColorKey = "color";
    private const string connectionLineWidthKey = "width";

    private UpdatePosition updatePosition;
    private LineRenderer lineRenderer;

    /// <summary>
    /// Sets the component up
    /// </summary>
    private void Awake()
    {
        updatePosition = GetComponent<UpdatePosition>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Deserializes the given SerializedObject and applies its values
    /// Expects the keys "start", "destination" in order to load the issue
    /// </summary>
    /// <param name="serializedObject">The SerializedObject with the save data</param>
    public async void Deserialize(SerializedObject serializedObject)
    {
        Vector3 startPos = serializedObject.Vector3s[startObjectPositionKey];
        Vector3 destinationPos = serializedObject.Vector3s[destinationObjectPositionKey];
        bool width = serializedObject.Bools[connectionLineWidthKey];
        Vector3 lineColorData = serializedObject.Vector3s[connectionLineColorKey];

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObjects)
        {
            if (go.activeInHierarchy)
            {
                if(go.transform.position == startPos)
                {
                    updatePosition.StartObject = go;
                }
                else if (go.transform.position == destinationPos)
                {
                    updatePosition.DestinationObject = go;
                }
            }
        }
        updatePosition.IsCurrentlyThick = width;
        if (width)
        {
            lineRenderer.startWidth = 0.04f;
            lineRenderer.endWidth = 0.04f;
        }
        else
        {
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
        }

        Color lineColor = new Color(lineColorData.x, lineColorData.y, lineColorData.z, 255);
        GetComponent<Renderer>().material.SetColor("_Color", lineColor);
    }

    /// <summary>
    /// Serializes the data of the given issue card into a SerializedObject
    /// Inserts values for "start", "destination"
    /// </summary>
    /// <returns>The SerializedObject with the values for the card</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Vector3s.Add(startObjectPositionKey, updatePosition.StartObject.transform.position);
        serializedObject.Vector3s.Add(destinationObjectPositionKey, updatePosition.DestinationObject.transform.position);
        serializedObject.Bools.Add(connectionLineWidthKey, updatePosition.IsCurrentlyThick);
        serializedObject.Vector3s.Add(connectionLineColorKey, new Vector3(
            GetComponent<Renderer>().material.color.r,
            GetComponent<Renderer>().material.color.g,
            GetComponent<Renderer>().material.color.b));
        return serializedObject;
    }
}
