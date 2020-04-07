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

    private UpdatePosition updatePosition;

    /// <summary>
    /// Sets the component up
    /// </summary>
    private void Awake()
    {
        updatePosition = GetComponent<UpdatePosition>();
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
        return serializedObject;
    }
}
