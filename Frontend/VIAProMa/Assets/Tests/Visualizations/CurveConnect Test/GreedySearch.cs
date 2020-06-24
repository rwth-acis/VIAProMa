using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using static IntTriple;
using static LineControllScriptFrameShare;

public class Greedy
{
    public static AStar.AStarResult<T> GreedySearch<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T, T, float> CostsBetween)
    {
        //SimplePriorityQueue<T> openSet = new SimplePriorityQueue<T>();
        //openSet.Enqueue(start,Heuristic(start));
        int count = 0;
        float pathCosts = 0;
        List<T> path = new List<T>();
        path.Add(start);
        T current = start;
        T cheapestNeigbor = start;
        float lowestCosts = float.PositiveInfinity;

        while (!GoalTest(current, goal) && count < 300)
        {
            List<T> neigbors = GetNeighbors(current);
            foreach (T neighbor in neigbors)
            {
                float costs = Heuristic(neighbor);
                if (costs < lowestCosts && !path.Contains(neighbor))
                {
                    cheapestNeigbor = neighbor;
                    lowestCosts = costs;
                }
            }
            lowestCosts = float.PositiveInfinity;
            pathCosts += CostsBetween(current,cheapestNeigbor);
            current = cheapestNeigbor;
            path.Add(current);
            count++;
        }
        return new AStar.AStarResult<T>(path, pathCosts);
    }
}
