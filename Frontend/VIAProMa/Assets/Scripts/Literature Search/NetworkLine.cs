using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing a line in a citation network.
/// </summary>
public class NetworkLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    [SerializeField] private float speed = 2;

    /// <summary>
    /// The DOI of the start node.
    /// </summary>
    public string StartDOI { get; private set; }
    /// <summary>
    /// The DOI of the target node.
    /// </summary>
    public string EndDOI { get; private set; }

    /// <summary>
    /// Transform of the start node.
    /// </summary>
    private Transform _start;
    /// <summary>
    /// Transform of the target node.
    /// </summary>
    private Transform _target;

    /// <summary>
    /// Updates the texture offset as well as the positions of the start and finish of the line each frame.
    /// </summary>
    void Update()
    {
        lineRenderer.material.mainTextureOffset = - Vector2.right * Time.time * speed;
        lineRenderer.SetPosition(0, _start.position);
        lineRenderer.SetPosition(1, _target.position);
    }

    /// <summary>
    /// Sets the transforms for start and finish positions of the line as well as the DOIs for the corresponding papers.
    /// </summary>
    /// <param name="start">Transform of the start node.</param>
    /// <param name="startDOI">DOI of the start node.</param>
    /// <param name="target">Transform of the target node.</param>
    /// <param name="endDOI">DOI of the target node.</param>
    public void SetLine(Transform start, string startDOI, Transform target, string endDOI)
    {
        // Set the global variables
        StartDOI = startDOI;
        EndDOI = endDOI;
        _start = start;
        _target = target;
        lineRenderer = GetComponent<LineRenderer>(); 

        // Initialize the start and finish positions.
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, target.position);

        // Prevent the sprite from streching.
        float width = lineRenderer.startWidth;
        lineRenderer.material.mainTextureScale = new Vector2(1f / width, 1.0f);
    }

    /// <summary>
    /// Changes the color of the line to <paramref name="color"/>.
    /// </summary>
    /// <param name="color">New color of the line.</param>
    public void ChangeColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
}
