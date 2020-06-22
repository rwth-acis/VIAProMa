using System;
using System.Collections.Generic;
using UnityEngine;

public class Entrance
{
    public Vector3 position
    { get; }
    //public List<Edge> edgesOld
    //{ get; }

    public Dictionary<Entrance, float> edges
    { get; }

    public Entrance(Vector3 position)
    {
        this.position = position;
        //edgesOld = new List<Edge>();
        edges = new Dictionary<Entrance, float>();
    }
}
