using System;
using System.Collections.Generic;
using UnityEngine;

public class Entrance
{
    public Vector3 position
    { get; }
    public List<Edge> edges
    { get; }

    public Entrance(Vector3 position)
    {
        this.position = position;
        edges = new List<Edge>();
    }
}
