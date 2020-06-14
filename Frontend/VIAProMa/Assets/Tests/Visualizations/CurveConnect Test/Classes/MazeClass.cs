using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private Dictionary<IntTriple,Cluster> clusters;

    public Maze(float distanceStartGoal, float stepSize)
    {
        clusters = new Dictionary<IntTriple, Cluster>();
    }

    public void addCluster(IntTriple clusterNumber)
    {
        Cluster newCluster = new Cluster();

        //check for already computed entrances of adjacent clusters (not in diaginal direction)
        IntTriple offset = new IntTriple(-1, -1, -1);
        List<Vector3> entrances;
        for (int x = -1; x <= 1; x += 2)
        {
            IntTriple xOffset = new IntTriple(x,0,0);
            entrances = clusters[clusterNumber + xOffset].getEntrances(xOffset);
            if (entrances != null)
            {
                newCluster.setEntrances(xOffset * -1, entrances);
            }
        }

        for (int y = -1; y <= 1; y += 2)
        {

        }

        for (int z = -1; z <= 1; z += 2)
        {

        }

    }
}
