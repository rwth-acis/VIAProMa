using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Reprents a generic data point which can be used in diagrams
/// </summary>
public class DataPoint
{
    public Vector3 position;
    public Color color;

    /// <summary>
    /// Creates a data point object
    /// </summary>
    /// <param name="position">The position of the data point</param>
    /// <param name="color">The color of the data point</param>
    public DataPoint(Vector3 position, Color color)
    {
        this.position = position;
        this.color = color;
    }
}
