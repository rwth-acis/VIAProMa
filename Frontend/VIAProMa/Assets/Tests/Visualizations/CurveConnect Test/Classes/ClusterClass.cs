using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    //The entraces on the cluster side (1,0,0) for example would be the entraces on the plane that is in direction (1,0,0) from the middle of the cluster.
    private Dictionary<IntTriple, List<Vector3>> clusterSides;

    public List<Vector3> getEntrances(IntTriple planeDirection)
    {
        if (clusterSides.ContainsKey(planeDirection))
            return clusterSides[planeDirection];
        else
            return null;
    }

    public void setEntrances(IntTriple planeDirection, List<Vector3> entranceList)
    {
        clusterSides.Add(planeDirection,entranceList);
    }
}
