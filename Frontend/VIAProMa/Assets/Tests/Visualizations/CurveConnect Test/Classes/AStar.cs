using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
//using static LineControllScriptFrameShare;


public class AStar : GridSearch
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
        totalPath.Reverse();
        return totalPath;
    }



    public static IEnumerator AStarSearchCoroutine(IntTriple start, IntTriple goal, Func<IntTriple, List<IntTriple>> GetNeighbors, Func<IntTriple, IntTriple, bool> GoalTest, Func<IntTriple, float> Heuristic, Func<IntTriple, IntTriple, float> CostsBetween, CoroutineData data, bool calculatePath = true)
    {
        SimplePriorityQueue<IntTriple> openSet = new SimplePriorityQueue<IntTriple>();
        Dictionary<IntTriple, IntTriple> cameFrom = new Dictionary<IntTriple, IntTriple>();
        Dictionary<IntTriple, float> gScore = new Dictionary<IntTriple, float>();
        openSet.Enqueue(start, 0);
        gScore.Add(start, Heuristic(start));
        IntTriple current;
        DateTime timeAtBeginOfFrame = DateTime.Now;
        int frameCount = 0;

        while (openSet.Count != 0)
        {
            if ((DateTime.Now - timeAtBeginOfFrame).TotalMilliseconds > 10)
            {
                frameCount++;
                if (frameCount > 10)
                {
                    Debug.Log("Too Long");
                    data.status = CoroutineStatus.Failure;
                    yield break;
                }
                yield return null;
                timeAtBeginOfFrame = DateTime.Now;
            }

            current = openSet.Dequeue();
            if (GoalTest(current, goal))
            {
                List<IntTriple> optimalPath = null;
                if (calculatePath)
                    optimalPath = AStar.reconstruct_path<IntTriple>(cameFrom, current);

                Debug.Log(frameCount);
                data.output = new AStar.SearchResult<IntTriple>(optimalPath, gScore[current]);
                data.status = CoroutineStatus.Finished;
                yield break;
            }

            List<IntTriple> neighbors = GetNeighbors(current);

            //TODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
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
            data.status = CoroutineStatus.Failure;
            yield break;
        }
        data.output = new AStar.SearchResult<IntTriple>(null, float.PositiveInfinity);
        data.status = CoroutineStatus.Failure;
    }

    public static IEnumerator AStarGridSearchCoroutine(IntTriple startCell, IntTriple goalCell, float stepSize, GameObject startObject, GameObject goalObject, CoroutineData data)
    {
        return AStarSearchCoroutine(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize, startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalObject.transform.position, stepSize), CostsBetweenGeneratorGrid(stepSize), data);
    }

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
                    optimalPath = reconstruct_path<T>(cameFrom, current);

                return new SearchResult<T>(optimalPath, gScore[current]);
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
            return new SearchResult<T>(new List<T>(), float.PositiveInfinity);
        }
        return new SearchResult<T>(null, float.PositiveInfinity);
    }

    public static SearchResult<IntTriple> AStarGridSearch(IntTriple startCell, IntTriple goalCell, float stepSize, Vector3 goalPosition, GameObject startObject, GameObject goalObject)
    {
        return AStarSearch<IntTriple>(startCell, goalCell, GetNeighborsGeneratorGrid(stepSize,startObject, goalObject), (x, y) => x == y, HeuristicGeneratorGrid(goalPosition, stepSize), CostsBetweenGeneratorGrid(stepSize));
    }


}

//Necassary for the coroutine, because StartCoroutine only allows one parameter
public class CoroutineData
{
    //Extra parameters for controling the coroutine
    public CoroutineStatus status;
    public AStar.SearchResult<IntTriple> output;
    public Vector3[] curve;



    public CoroutineData() {
        status = CoroutineStatus.NoHandler;
    }
}

public enum CoroutineStatus
{
    Running,
    Failure,
    Finished,
    Waiting,
    NoHandler
}
