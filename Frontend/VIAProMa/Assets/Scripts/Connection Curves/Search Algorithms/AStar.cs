﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Threading.Tasks;

/// <summary>
/// A class for AStar search that is independent from the graph implementation. Contains an implementation specialised for grid search.
/// </summary>
public class AStar : GridSearch
{
    /// <summary>
    /// Starts an AStar search. The graph needs to be provided through the GetNeighbors function.
    /// </summary>
    public static SearchResult<T> AStarSearch<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T, T, float> CostsBetween, bool calculatePath = true)
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
                    optimalPath = ReconstructPath<T>(cameFrom, current);

                return new SearchResult<T>(optimalPath, gScore[current]);
            }

            List<T> neighbors = GetNeighbors(current);

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
            return new SearchResult<T>(new List<T>(), float.PositiveInfinity);
        }
        return new SearchResult<T>(null, float.PositiveInfinity);
    }

    /// <summary>
    /// Starts an AStar search on a 3D grid.
    /// </summary>
    public static SearchResult<IntTriple> AStarGridSearch(IntTriple startCell, IntTriple goalCell, float stepSize, Vector3 goalPosition, GameObject startObject, GameObject goalObject)
    {
        return AStarSearch<IntTriple>(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize,startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalPosition, stepSize), CostsBetweenGeneratorGrid(stepSize));
    }

    /// <summary>
    /// Starts an async AStar search. The graph needs to be provided through the GetNeighbors function. The search waits a frame, when it needed more than 7 ms and cancels the search if it needs more than 10 frames.
    /// </summary>
    public static async Task<SearchResult<T>> AStarSearchAsync<T>(T start, T goal, Func<T, List<T>> GetNeighbors, Func<T, T, bool> GoalTest, Func<T, float> Heuristic, Func<T, T, float> CostsBetween, bool calculatePath = true)
    {
        SimplePriorityQueue<T> openSet = new SimplePriorityQueue<T>();
        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        Dictionary<T, float> gScore = new Dictionary<T, float>();
        openSet.Enqueue(start, 0);
        gScore.Add(start, Heuristic(start));
        T current;
        int frameCount = 0;
        DateTime timeAtBeginOfFrame = DateTime.Now; ;

        while (openSet.Count != 0)
        {
            if ((DateTime.Now - timeAtBeginOfFrame).TotalMilliseconds > 7)
            {
                frameCount++;
                if (frameCount > 10)
                {
                    Debug.Log("Too Long");
                    return new SearchResult<T>(null,float.NaN);
                }
                await Task.Yield();
                timeAtBeginOfFrame = DateTime.Now;
            }

            current = openSet.Dequeue();
            if (GoalTest(current, goal))
            {
                List<T> optimalPath = null;
                if (calculatePath)
                    optimalPath = ReconstructPath<T>(cameFrom, current);

                return new SearchResult<T>(optimalPath, gScore[current]);
            }

            List<T> neighbors = GetNeighbors(current);

            foreach (T neighbor in neighbors)
            {
                float h = Heuristic(neighbor);
                float tentative_gScore = gScore[current] + CostsBetween(current, neighbor);
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
            return new SearchResult<T>(new List<T>(), float.PositiveInfinity);
        }
        return new SearchResult<T>(null, float.PositiveInfinity);
    }

    /// <summary>
    /// Starts an async AStar search on a 3D grid. The search waits a frame, when it needed more than 7 ms and cancels the search if it needs more than 10 frames.
    /// </summary>
    public static Task<SearchResult<IntTriple>> AStarGridSearchAsync(IntTriple startCell, IntTriple goalCell, float stepSize, GameObject startObject, GameObject goalObject)
    {
        return AStarSearchAsync(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize, startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalObject.transform.position, stepSize), CostsBetweenGeneratorGrid(stepSize));
    }

    /// <summary>
    /// Generates the path from cameFrom dictonary and the last visited node.
    /// </summary>
    private static List<T> ReconstructPath<T>(Dictionary<T, T> cameFrom, T current)
    {
        List<T> totalPath = new List<T>();
        totalPath.Add(current);

        T ancestor;
        while (cameFrom.TryGetValue(current, out ancestor))
        {
            totalPath.Add(ancestor);
            current = ancestor;
        }
        totalPath.Reverse();
        return totalPath;
    }
}