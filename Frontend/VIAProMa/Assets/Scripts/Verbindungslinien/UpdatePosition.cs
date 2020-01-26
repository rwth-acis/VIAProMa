using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    /// <summary>
    /// Reference to the start object of the line
    /// </summary>
    [HideInInspector] public GameObject startObject;

    /// <summary>
    /// Reference to the destination object of the line
    /// </summary>
    [HideInInspector] public GameObject destinationObject;

    /// <summary>
    /// Reference to the LineRenderer component
    /// </summary>
    private LineRenderer lineRendererComponent;

    /// <summary>
    /// Getting the Reference to the LineRenderer component
    /// </summary>
    void Start()
    {
        lineRendererComponent = transform.GetComponent<LineRenderer>();
    }

    /// <summary>
    /// Always update the position of the line according to the objects' position
    /// </summary>
    void Update()
    {
        if(startObject == null || destinationObject == null)
        {
            Destroy(gameObject);
        }
        lineRendererComponent.SetPosition(0, startObject.transform.position);
        lineRendererComponent.SetPosition(1, destinationObject.transform.position);
    }
}
