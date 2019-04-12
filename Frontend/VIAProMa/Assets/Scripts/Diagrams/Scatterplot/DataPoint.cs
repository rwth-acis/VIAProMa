using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint
{
    public Vector3 position;
    public Color color;

    public DataPoint(Vector3 position, Color color)
    {
        this.position = position;
        this.color = color;
    }
}
