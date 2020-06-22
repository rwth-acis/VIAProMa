using System;
using System.Collections.Generic;
using UnityEngine;
using static LineControllScriptFrameShare;
using static System.Math;
using static IntTriple;

public class Maze
{
    private Dictionary<IntTriple, Cluster> clusters;
    private float stepSize; //how big are the cells
    private int clusterSize; //how many cells are in one edge of a cluster


    public Maze(float stepSize, int clusterSize)
    {
        clusters = new Dictionary<IntTriple, Cluster>();
        this.stepSize = stepSize;
        this.clusterSize = clusterSize;
    }

    //Iterates through the triples like (-1,0,0) -> (1,0,0) -> (0,-1,0) -> (0,1,0) ...
    private IntTriple TripleIterator(IntTriple triple)
    {
        if (triple.x < 0 || triple.y < 0 || triple.z < 0)
            return (triple * -1);
        else
            return new IntTriple(0, -triple.x, -triple.y);
    }

    Func<IntTriple, List<IntTriple>> GetNeighborsFunctionGenerator(IntTriple clusterNumber)
    {
        List<IntTriple> getNeighbors(IntTriple node)
        {
            List<IntTriple> neighbors = new List<IntTriple>();


            for (int x = -1; x <= 1; x += 1)
            {
                for (int y = -1; y <= 1; y += 1)
                {
                    for (int z = -1; z <= 1; z += 1)
                    {
                        IntTriple cell = new IntTriple(node.x + x, node.y + y, node.z + z);
                        /*
                        if ((x != 0 || y != 0 || z != 0))
                        {
                            Debug.Log("jo");
                        }
                        IntTriple test = cell / clusterSize;
                        if (test == clusterNumber)
                        {
                            Debug.Log("Jo");
                        }
                        */
                        if ((x != 0 || y != 0 || z != 0) //dont return the node as its own neighbor
                            && clusterNumber == CellToCluster(cell,clusterSize)) //stay in the same cluster
                        {
                            if (!collisonWithObstacle(CellToVector(cell, stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2)))
                            {
                                neighbors.Add(cell);
                            }
                        }
                    }
                }
            }
            return neighbors;
        }

        return getNeighbors;

    }

    public void addCluster(IntTriple clusterNumber)
    {
        if (clusters.ContainsKey(clusterNumber))
            return;

        Cluster newCluster = new Cluster();

        //Set the entrance for every site

        for (IntTriple x = new IntTriple(-1, 0, 0); x != new IntTriple(0, 0, 0); x = TripleIterator(x))
        {
            setUniqueEntrances(newCluster, clusterNumber, x);
        }



        //calculate edges between entrances
        for (IntTriple x = new IntTriple(-1, 0, 0); x != new IntTriple(0, 0, 1); x = TripleIterator(x))
        {
            for (IntTriple y = TripleIterator(x); y != new IntTriple(0, 0, 0); y = TripleIterator(y))
            {
                foreach (Entrance entranceX in newCluster.getEntrances(x))
                {
                    foreach (Entrance entranceY in newCluster.getEntrances(y))
                    {
                        float costs = AStar.AStarSearch<IntTriple>(VectorToCell(entranceX.position + CellToVector(x * -1, stepSize / 2),stepSize), VectorToCell(entranceY.position + CellToVector(y * -1, stepSize / 2),stepSize), 
                            stepSize, GetNeighborsFunctionGenerator(clusterNumber), (item1,item2) => item1==item2, LineControllScriptFrameShare.HeuristicGenerator(entranceY.position, stepSize), 
                            LineControllScriptFrameShare.CostsBetweenGenerator(stepSize), false).costs;
                        entranceX.edges.Add(new Edge(entranceY, costs));
                        entranceY.edges.Add(new Edge(entranceX, costs));
                    }
                }
            }
        }

        clusters.Add(clusterNumber, newCluster);
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
            Vector3 directionVec = new Vector3(direction.x,direction.y,direction.z);
            Vector3 directionVecMask = new Vector3(Abs(directionVec.x), Abs(directionVec.y), Abs(directionVec.z));
            float clusterLength = clusterSize * stepSize;
            cluster.setEntrances(direction,
                 calculateEntrances(CellToVector(clusterNumber, clusterLength) + directionVec * clusterLength / 2, //This is the middle of the clusterside in direction 'direction'
                 (directionVecMask * stepSize) / 2 + (new Vector3(1, 1, 1) - directionVecMask) * clusterLength, //This results in a cube with the length 'stepSize/2' in direction 
                                                                                                                //'direction' and the length 'clusterLength' in the other two orthogonal
                                                                                                                //directions. It will be used to scan for obstacles on the cluster side
                 direction, stepSize)
                 );
        }
    }

    //Recursevly calculates the entrances with a given scan box, that gets divided verticaly/horizontal until the box
    //doesn't contain an obstacle or until every side of the box is smaller than the step size. 
    private List<Entrance> calculateEntrances(Vector3 center, Vector3 boxSize, IntTriple normal, float stepSize)
    {
        List<Entrance> entrances = new List<Entrance>();
        calculateEntrancesHelper(center, boxSize, normal, stepSize, true, entrances);
        return entrances;
    }

    private void calculateEntrancesHelper(Vector3 center, Vector3 boxSize, IntTriple normal, float stepSize, bool cutVertically, List<Entrance> entrances)
    {
        if (collisonWithObstacle(center, boxSize / 2))
        {
            Vector3 normalOrthogonal = new Vector3();
            //Rotates the normal in the local coordinate system to the right(if cutVertical) or up (if !cutVertical). I am not using a rotation Matrix/Qaternion because it would be to slow.
            /*if (cutVertically)
            {
                if (normal.y == 0)
                    normalOrthogonal = new Vector3(normal.z, normal.x, 0);
                else
                    normalOrthogonal = new Vector3(0, 1, 0);
            }
            else
            {
                if (normal.x == 0)
                    normalOrthogonal = new Vector3(0, normal.z, normal.y);
                else
                    normalOrthogonal = new Vector3(0, 0, 1);
            }
            */

            Vector3 normalVec = new Vector3(normal.x, normal.y, normal.z);
            /*
            if (normal.y == 0)
            {
                if (cutVertically)
                    normalOrthogonal = Quaternion.Euler(0, 90, 0) * normalVec;
                else
                    normalOrthogonal = new Vector3(0, 1, 0);
            }
            else
            {
                if (cutVertically)
                    //normalOrthogonal = Quaternion.Euler(0, 0, 90) * normalVec;
                    normalOrthogonal = new Vector3(1,0,0);
                else
                    //normalOrthogonal = Quaternion.Euler(90, 0, 0) * normalVec;
                    normalOrthogonal = new Vector3(0, 0, 1);
            }
            */

            if (cutVertically)
            {
                if (normal.x != 0)
                    normalOrthogonal = new Vector3(0, 0, 1);
                else
                    normalOrthogonal = new Vector3(1, 0, 0);
            }
            else
            {
                if (normal.y == 0)
                    normalOrthogonal = new Vector3(0, 1, 0);
                else
                    normalOrthogonal = new Vector3(0,0,1);
            }
           
            Vector3 normalOrthogonalMask = new Vector3(Abs(normalOrthogonal.x), Abs(normalOrthogonal.y), Abs(normalOrthogonal.z));
            Vector3 boxScaling = normalOrthogonalMask / 2;
            boxScaling = new Vector3(1, 1, 1) - boxScaling;
            boxSize.Scale(boxScaling);
            Vector3 boxSiceCopy = boxSize;
            boxSiceCopy.Scale(normalOrthogonalMask);

            //Will be used to determine the new centers
            float quarterBoxLengthInDirection = boxSiceCopy.magnitude / 2;

            if (boxSize.x <= stepSize && boxSize.y <= stepSize && boxSize.z <= stepSize)
                return;
            else
            {
                calculateEntrancesHelper(center + normalOrthogonal * quarterBoxLengthInDirection, boxSize, normal, stepSize, !cutVertically, entrances);
                calculateEntrancesHelper(center + normalOrthogonal * -quarterBoxLengthInDirection, boxSize, normal, stepSize, !cutVertically, entrances);
            }
        }
        else
        {
            entrances.Add(new Entrance(center));
        }
    }

    //For inserting start and goal into the maze.
    public void InsertStartOrGoalNode(Vector3 position, bool start)
    {
        position /= stepSize * clusterSize;
        IntTriple clusterNumber = new IntTriple((int)position.x, (int)position.y, (int)position.z);
        addCluster(clusterNumber);
        Cluster cluster = clusters[clusterNumber];

        Entrance newNode = new Entrance(position);

        for (IntTriple x = new IntTriple(-1, 0, 0); x != new IntTriple(0, 0, 0); x = TripleIterator(x))
        {
            foreach (Entrance entrance in cluster.getEntrances(x))
            {
                float costs = AStar.AStarSearch(entrance.position + CellToVector(x * -1, stepSize / 2), position, stepSize, GetNeighborsFunctionGenerator(clusterNumber), false).costs;
                newNode.edges.Add(new Edge(entrance, costs));
                entrance.edges.Add(new Edge(newNode, costs));
            }
        }
        if (start)
            cluster.start = newNode;
        else
            cluster.goal = newNode;
            
    }
}
