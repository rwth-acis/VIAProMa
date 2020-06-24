using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using static IntTriple;
using static LineControllScriptFrameShare;


public class AStar
{
    public static List<T> reconstruct_path<T>(Dictionary<T, T> cameFrom, T current)
    {
        List<T> totalPath = new List<T>();
        totalPath.Add(current);

        T ancestor;
        while (cameFrom.TryGetValue(current, out ancestor))
        {
            totalPath.Add(ancestor);
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

    public struct AStarResult<T>
    {
        public List<T> path { get; set; }
        public float costs { get; set; }

        public AStarResult(List<T> path, float length)
        {
            this.path = path;
            this.costs = length;
        }
    }


    public static AStarResult<T> AStarSearch<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T,T,float> CostsBetween, bool calculatePath = true)
    {


        SimplePriorityQueue<T> openSet = new SimplePriorityQueue<T>();
        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        Dictionary<T, float> gScore = new Dictionary<T, float>();
        openSet.Enqueue(start, 0);
        gScore.Add(start, Heuristic(start));
        T current;

        while (openSet.Count != 0)
        {
            current = openSet.Dequeue();
            if (GoalTest(current,goal))
            {
                List<T> optimalPath = null;
                if (calculatePath)
                    optimalPath = reconstruct_path<T>(cameFrom, current);

                return new AStarResult<T>(optimalPath, gScore[current]);
            }

            List<T> neighbors = GetNeighbors(current);

            //TODO Maby here multithreading?
            foreach (T neighbor in neighbors)
            {
                float h = Heuristic(neighbor);
                float tentative_gScore = gScore[current] + CostsBetween(current,neighbor);
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
            return new AStarResult<T>(new List<T>(), float.PositiveInfinity);
        }
        return new AStarResult<T>(null, float.PositiveInfinity);
    }

}
