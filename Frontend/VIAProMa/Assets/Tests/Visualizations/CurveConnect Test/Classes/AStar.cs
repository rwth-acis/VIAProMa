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



    public static IEnumerator AStarSearchCoroutine(AStarParameter parameter)
    {
        SimplePriorityQueue<IntTriple> openSet = new SimplePriorityQueue<IntTriple>();
        Dictionary<IntTriple, IntTriple> cameFrom = new Dictionary<IntTriple, IntTriple>();
        Dictionary<IntTriple, float> gScore = new Dictionary<IntTriple, float>();
        openSet.Enqueue(parameter.start, 0);
        gScore.Add(parameter.start, parameter.Heuristic(parameter.start));
        IntTriple current;
        parameter.status = AStarStatus.Running;
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
                    parameter.status = AStarStatus.Failure;
                    yield break;
                }
                yield return null;
                timeAtBeginOfFrame = DateTime.Now;
            }

            current = openSet.Dequeue();
            if (parameter.GoalTest(current, parameter.goal))
            {
                List<IntTriple> optimalPath = null;
                if (parameter.calculatePath)
                    optimalPath = AStar.reconstruct_path<IntTriple>(cameFrom, current);

                parameter.output = new AStar.SearchResult<IntTriple>(optimalPath, gScore[current]);
                parameter.status = AStarStatus.Finished;
                yield break;
            }

            List<IntTriple> neighbors = parameter.GetNeighbors(current);

            //IntTripleODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
            {
                float h = parameter.Heuristic(neighbor);
                float tentative_gScore = gScore[current] + parameter.CostsBetween(current, neighbor);
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
            parameter.output = new AStar.SearchResult<IntTriple>(new List<IntTriple>(), float.PositiveInfinity);
            parameter.status = AStarStatus.Finished;
            yield break;
        }
        parameter.output = new AStar.SearchResult<IntTriple>(null, float.PositiveInfinity);
        parameter.status = AStarStatus.Finished;
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
public class AStarParameter
{
    public IntTriple start;
    public IntTriple goal;
    public Func<IntTriple, List<IntTriple>> GetNeighbors;
    public Func<IntTriple, IntTriple, bool> GoalTest;
    public Func<IntTriple, float> Heuristic;
    public Func<IntTriple, IntTriple, float> CostsBetween;
    public bool calculatePath;

    //Extra parameters for controling the coroutine
    public AStarStatus status;
    public AStar.SearchResult<IntTriple> output;
    public Vector3[] curve;

    public AStarParameter(IntTriple start, IntTriple goal, Func<IntTriple, List<IntTriple>> GetNeighbors, Func<IntTriple, IntTriple, bool> GoalTest, Func<IntTriple, float> Heuristic, Func<IntTriple, IntTriple, float> CostsBetween, bool calculatePath = true)
    {
        this.start = start;
        this.goal = goal;
        this.GetNeighbors = GetNeighbors;
        this.GoalTest = GoalTest;
        this.Heuristic = Heuristic;
        this.CostsBetween = CostsBetween;
        this.calculatePath = calculatePath;
        status = AStarStatus.Waiting;
    }

    public AStarParameter(IntTriple startCell, IntTriple goalCell, float stepSize, Vector3 goalPosition, GameObject startObject, GameObject goalObject)
    {
        start = startCell;
        goal = goalCell;
        GetNeighbors = AStar.GetNeighborsGeneratorGrid(stepSize, startObject, goalObject);
        GoalTest = (x, y) => x == y;
        Heuristic = AStar.HeuristicGeneratorGrid(goalPosition, stepSize);
        CostsBetween = AStar.CostsBetweenGeneratorGrid(stepSize);
        calculatePath = true;
        status = AStarStatus.Waiting;
    }

    public AStarParameter() {
        status = AStarStatus.NoHandler;
    }
}

public enum AStarStatus
{
    Running,
    Failure,
    Finished,
    Waiting,
    NoHandler
}
