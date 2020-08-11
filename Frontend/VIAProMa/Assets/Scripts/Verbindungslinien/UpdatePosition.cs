using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    /// <summary>
    /// Reference to the start object of the line
    /// </summary>
    private GameObject _startObject;
    public GameObject StartObject
    {
        get { return _startObject; }
        set { _startObject = value; }
    }

    /// <summary>
    /// Reference to the destination object of the line
    /// </summary>
    private GameObject _destinationObject;
    public GameObject DestinationObject
    {
        get { return _destinationObject; }
        set { _destinationObject = value; }
    }

    /// <summary>
    /// Currently selected line width
    /// </summary>
    private bool _isCurrentlyThick;
    public bool IsCurrentlyThick
    {
        get { return _isCurrentlyThick; }
        set { _isCurrentlyThick = value; }
    }

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
        if (lineRendererComponent.startWidth == 0.04f)
        {
            IsCurrentlyThick = true;
        }
        else
        {
            IsCurrentlyThick = false;
        }
    }

    /// <summary>
    /// Always update the position of the line according to the objects' position
    /// </summary>
    void Update()
    {
        if (_startObject == null || _destinationObject == null)
        {
            Destroy(gameObject);
        }
        lineRendererComponent.SetPosition(0, _startObject.transform.position);
        lineRendererComponent.SetPosition(1, _destinationObject.transform.position);
    }
}
