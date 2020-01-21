using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Serializes the data of the connecting lines
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LinesSerializer : MonoBehaviour, ISerializable
{

    /// <summary>
    /// The globally unique identifier (GUID) of the start object
    /// </summary>
    private const string startKey = "startKey";

    /// <summary>
    /// The globally unique identifier (GUID) of the destination object
    /// </summary>
    private const string destinationKey = "destinationKey";

    /// <summary>
    /// The line to be de/serialized
    /// </summary>
    private LineRenderer line;

    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    public GameObject lineRendererPrefab;


    /// <summary>
    /// The start object of the line
    /// </summary>
    [HideInInspector] public GameObject start;

    /// <summary>
    /// The destination object of the line
    /// </summary>
    [HideInInspector] public GameObject destination;

    /// <summary>
    /// Initializes the component
    /// </summary>
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(LineDrawLogic), gameObject);
        }
    }

  


    /// <summary>
    /// Applies the settings which are stored in the serializedObject to the connecting lines
    /// </summary>
    /// <param name="serializedObject">The serialized object with the save data</param>
    public void Deserialize(SerializedObject serializedObject)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        String startId = serializedObject.Strings[startKey];
        String destinationId = serializedObject.Strings[destinationKey];

        foreach (GameObject gameObject in allObjects)
        { 
            if (gameObject.GetComponent<Serializer>())
            {


                if (gameObject.GetComponent<Serializer>().Id == startId)
                {
                    start = gameObject;
                    line.GetComponent<UpdatePosition>().startObject = start;
                    line.GetComponent<LineRenderer>().SetPosition(0, start.transform.position);

                }
                if (gameObject.GetComponent<Serializer>().Id == destinationId)
                {
                    destination = gameObject;
                    line.GetComponent<UpdatePosition>().destinationObject = destination;
                    line.GetComponent<LineRenderer>().SetPosition(1, destination.transform.position);

                }
                if (start != null && destination != null)
                {
                    break;
                }

                if (line.GetComponent<LineRenderer>().GetPosition(0) == null || line.GetComponent<LineRenderer>().GetPosition(1) == null)
                {
                    SpecialDebugMessages.LogComponentNotFoundError(this, nameof(LineDrawLogic), gameObject);
                }
            }
        }

    }


   
    /// <summary>
    /// Writes the data of the progress bar into a save data object
    /// </summary>
    /// <returns>The saved data of the building progress bar component</returns>
    public SerializedObject Serialize()
    {
        SerializedObject serializedObject = new SerializedObject();
        serializedObject.Strings.Add(startKey, start.GetComponent<Serializer>().Id);
        serializedObject.Strings.Add(destinationKey, destination.GetComponent<Serializer>().Id);

        return serializedObject;
    }
}
