﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Threading.Tasks;

public class Greedy : GridSearch
{
    public static SearchResult<T> GreedySearch<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T, T, float> CostsBetween)
    {
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
            pathCosts += CostsBetween(current, cheapestNeigbor);
            current = cheapestNeigbor;
            path.Add(current);
            count++;
        }
        return new SearchResult<T>(path, pathCosts);
    }

    public static Vector3[] postProcessing(Vector3[] path, float scanCubeSize)
    {
        List<Vector3> refinedPath = new List<Vector3>();

        Vector3 lowerLeftCorner = new Vector3();
        Vector3 upperRigthCorner = new Vector3();

        foreach (Vector3 point in path)
        {
            lowerLeftCorner = new Vector3(point.x - scanCubeSize / 2, point.y - scanCubeSize / 2, point.z);
            upperRigthCorner = lowerLeftCorner + new Vector3(scanCubeSize, scanCubeSize, scanCubeSize);

            int pointCount = 0;
            foreach (Vector3 point2 in path)
            {
                if (point2.x >= lowerLeftCorner.x && point2.y >= lowerLeftCorner.y && point2.z >= lowerLeftCorner.z
                   && point2.x <= upperRigthCorner.x && point2.y <= upperRigthCorner.y && point2.z <= upperRigthCorner.z)
                    pointCount++;
            }
            if (pointCount <= 3)
            {
                refinedPath.Add(point);
            }
            pointCount = 0;
        }

        return refinedPath.ToArray();
    }

    public static SearchResult<IntTriple> GreedyGridSearch(IntTriple startCell, IntTriple goalCell, float stepSize, Vector3 goalPosition, GameObject startObject, GameObject goalObject)
    {
        return GreedySearch<IntTriple>(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize, startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalPosition, stepSize), CostsBetweenGeneratorGrid(stepSize));
    }

    public static async Task<SearchResult<T>> GreedySearchAsync<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T, T, float> CostsBetween, bool calculatePath = true)
    {
        float pathCosts = 0;
        List<T> path = new List<T>();
        path.Add(start);
        T current = start;
        T cheapestNeigbor = start;
        float lowestCosts = float.PositiveInfinity;
        int frameCount = 0;
        DateTime timeAtBeginOfFrame = DateTime.Now;

        while (!GoalTest(current, goal))
        {
            if ((DateTime.Now - timeAtBeginOfFrame).TotalMilliseconds > 7)
            {
                frameCount++;
                if (frameCount > 3)
                {
                    Debug.Log("Too Long");
                    return new SearchResult<T>(null, float.NaN);
                }
                await Task.Yield();
                timeAtBeginOfFrame = DateTime.Now;
            }

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
            pathCosts += CostsBetween(current, cheapestNeigbor);
            current = cheapestNeigbor;
            path.Add(current);
        }
        return new SearchResult<T>(path, pathCosts);
    }
    public static Task<SearchResult<IntTriple>> GreedyGridSearchAsync(IntTriple startCell, IntTriple goalCell, float stepSize, GameObject startObject, GameObject goalObject)
    {
        return GreedySearchAsync(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize, startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalObject.transform.position, stepSize), CostsBetweenGeneratorGrid(stepSize));
    }
}