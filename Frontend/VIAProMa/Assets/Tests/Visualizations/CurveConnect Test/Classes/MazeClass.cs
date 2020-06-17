using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private Dictionary<IntTriple, Cluster> clusters;

    public Maze(float distanceStartGoal, float stepSize)
    {
        clusters = new Dictionary<IntTriple, Cluster>();
    }

    public void addCluster(IntTriple clusterNumber)
    {
        Cluster newCluster = new Cluster();

        //check for already computed entrances of adjacent clusters (not in diagonal direction)
        for (int x = -1; x <= 1; x += 2)
        {
            setUniqueEntrances(newCluster, clusterNumber, new IntTriple(x, 0, 0));
        }

        for (int y = -1; y <= 1; y += 2)
        {
            setUniqueEntrances(newCluster, clusterNumber, new IntTriple(0, y, 0));
        }

        for (int z = -1; z <= 1; z += 2)
        {
            setUniqueEntrances(newCluster, clusterNumber, new IntTriple(0, 0, z));
        }

    }

    //Sets an entrance only when it wasn't already set
    private void setUniqueEntrances(Cluster cluster, IntTriple clusterNumber, IntTriple direction)
    {
        //Was the adjacent cluster in direction "direction" already generated?
        //Yes => take the entrances from the adjacent cluster, because they were already calculated
        if (clusters.ContainsKey(clusterNumber + direction))
        {
            cluster.setEntrances(direction, clusters[clusterNumber + direction].getEntrances(direction * -1));
        }
        //No => calculate the entrances
        else
        {
            List<Vector3> entrances;

        }
    }

    //Recursevly calculates the entrances in a direction
    private void calculateEntrances(Vector3 middlePoint, float collisionSize, float minSize, bool cutVertically, List<Vector3> entrances)
    {

    }
}
