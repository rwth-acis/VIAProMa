using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using static IntTriple;
using static LineControllScriptFrameShare;


public class AStar
{
    public static List<Vector3> reconstruct_path(Dictionary<IntTriple, IntTriple> cameFrom, IntTriple current, float stepSize)
    {
        List<Vector3> totalPath = new List<Vector3>();
        totalPath.Add(CellToVector(current, stepSize));

        IntTriple ancestor;
        while (cameFrom.TryGetValue(current, out ancestor))
        {
            totalPath.Add(CellToVector(ancestor, stepSize));
            current = ancestor;
        }

        return totalPath;
    }

    public struct AStarResult
    {
        public List<Vector3> path { get; set; }
        public float costs { get; set; }

        public AStarResult(List<Vector3> path, float length)
        {
            this.path = path;
            this.costs = length;
        }
    }


    public static AStarResult AStarSearch(Vector3 start, Vector3 goal, float stepSize, Func<IntTriple, List<IntTriple>> getNeighbors, bool calculatePath = true)
    {


        SimplePriorityQueue<IntTriple> openSet = new SimplePriorityQueue<IntTriple>();
        Dictionary<IntTriple, IntTriple> cameFrom = new Dictionary<IntTriple, IntTriple>();
        Dictionary<IntTriple, float> gScore = new Dictionary<IntTriple, float>();
        IntTriple startCell = VectorToCell(start,stepSize);
        IntTriple goalCell = VectorToCell(goal, stepSize);
        openSet.Enqueue(startCell, 0);
        gScore.Add(startCell, Vector3.Distance(start, goal));
        IntTriple current;

        while (openSet.Count != 0)
        {
            current = openSet.Dequeue();
            if (current == goalCell)
            {
                List<Vector3> optimalPath = null;
                if (calculatePath)
                    optimalPath = reconstruct_path(cameFrom, current, stepSize);

                return new AStarResult(optimalPath, gScore[current]);
            }

            List<IntTriple> neighbors = getNeighbors(current);

            //TODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
            {
                float h = Vector3.Distance(CellToVector(neighbor, stepSize), goal);
                float tentative_gScore = gScore[current] + Vector3.Distance(CellToVector(current, stepSize), CellToVector(neighbor, stepSize));
                float neighboreGScore;
                if (gScore.TryGetValue(neighbor, out neighboreGScore))
                {
                    if (tentative_gScore < neighboreGScore)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        openSet.EnqueueWithoutDuplicates(neighbor, neighboreGScore + h);
                    }
                }
                //if neighbore dosn't have a gScore then it's infinit and therefore bigger than tentative_gScore
                else
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    openSet.EnqueueWithoutDuplicates(neighbor, tentative_gScore + h);
                }
            }
        }
        if (openSet.Count == 0)
        {
            //open set is empty and goal is never reached => no possible path
            return new AStarResult(new List<Vector3>(), float.PositiveInfinity);
        }
        return new AStarResult(null, float.PositiveInfinity);
    }
}
