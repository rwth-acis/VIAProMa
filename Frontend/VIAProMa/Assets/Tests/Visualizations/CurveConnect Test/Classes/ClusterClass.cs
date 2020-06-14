using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    //The entraces on (1,0,0) for example would be the entraces on the plane that is in direction (1,0,0) from the middle of the cluster.
    private Dictionary<IntTriple, List<Vector3>> entrances;

    public List<Vector3> getEntrances(IntTriple planeDirection)
    {
        if (entrances.ContainsKey(planeDirection))
            return entrances[planeDirection];
        else
            return null;
    }

    public void setEntrances(IntTriple planeDirection, List<Vector3> entranceList)
    {
        entrances.Add(planeDirection,entranceList);
    }
}
