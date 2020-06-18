using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LineControllScriptFrameShare;

public class Maze
{
    private Dictionary<IntTriple, Cluster> clusters;
    private int clusterSize; //how many cells are in one edge of a cluster
    private float stepSize; //how big are the cells

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
            Vector3 directionVec = tupleToVector(direction,1);
            float clusterLength = clusterSize * stepSize;
            cluster.setEntrances(direction,
                 calculateEntrances(tupleToVector(clusterNumber,clusterLength) + directionVec*clusterLength / 2, //This is the middle of the clusterside in direction 'direction'
                 (directionVec* stepSize)/2 + (new Vector3(1,1,1)-directionVec)* clusterLength, //This results in a cube with the length 'stepSize/2' in direction 'direction' and the length 'clusterLength' in the other two orthogonal directions
                 direction, stepSize)
                 );
        }
    }

    //Recursevly calculates the entrances with a given box
    private List<Vector3> calculateEntrances(Vector3 center, Vector3 boxSize, IntTriple normal, float stepSize)
    {
        List<Vector3> entrances = new List<Vector3>();
        calculateEntrancesHelper(center, boxSize, normal, stepSize, true, entrances);
        return entrances;
    }

    private void calculateEntrancesHelper(Vector3 center, Vector3 boxSize, IntTriple normal, float stepSize, bool cutVertically, List<Vector3> entrances)
    {
        

        if (collisonWithObstacle(center, boxSize / 2))
        {
            Vector3 normalOrthogonal;
            //Rotates the normal in the local coordinate system to the right(if verical) or up (if !vertical). I am not using a rotation Matrix/Qaternion because it would be to slow.
            if (cutVertically)
            {
                if (normal.y == 0)
                    normalOrthogonal = new Vector3(normal.z, normal.x, 0);
                else
                    normalOrthogonal = new Vector3(0, 1, 0);
            }
            else
            {
                if (normal.y == 0)
                    normalOrthogonal = new Vector3(0, 0, 1);
                else
                    normalOrthogonal = new Vector3(0,1,0);
            }
            Vector3 boxScaling = normalOrthogonal/2;
            Vector3 newBoxSize = new Vector3(boxSize.x * boxScaling.x, boxSize.y * boxScaling.y, boxSize.z * boxScaling.z);
            float quarterBoxLengthInDirection = (newBoxSize.x + newBoxSize.y + newBoxSize.z)/2;

            if (newBoxSize.x <= stepSize && newBoxSize.y <= stepSize && newBoxSize.z <= stepSize)
                return;
            else
            {
                calculateEntrancesHelper(center + normalOrthogonal * quarterBoxLengthInDirection, newBoxSize, normal, stepSize, !cutVertically, entrances);
                calculateEntrancesHelper(center + normalOrthogonal * -quarterBoxLengthInDirection, newBoxSize, normal, stepSize, !cutVertically, entrances);
            }
        }
        else
        {
            entrances.Add(center);
        }
    }
}
