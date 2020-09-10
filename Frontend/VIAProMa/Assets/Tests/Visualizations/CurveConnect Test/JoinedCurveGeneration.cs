using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class JoinedCurveGeneration : MonoBehaviour
{
    public float standartHeight = 0.2f;



    public static async Task UpdateAsync(List<ConnectionCurve> curves, float stepSize)
    {
        try
        {
            while (true)
            {
                //Check the standart curve and calculate the boundingboxes on the main thread
                BoundingBoxes[] boxes = new BoundingBoxes[curves.Count];
                for(int i = 0; i < curves.Count; i++)
                {
                    if (curves[i] != null)
                    {
                        //Try to use the standart curve
                        Vector3[] result = SimpleCurveGerneration.TryToUseStandartCurve(curves[i].start, curves[i].goal,60);
                        if (result != null)
                        {
                            curves[i].lineRenderer.positionCount = result.Length;
                            curves[i].lineRenderer.SetPositions(result);
                        }
                        else
                        {
                            boxes[i] = SimpleCurveGerneration.CalculateBoundingBoxes(curves[i].start, curves[i].goal);
                        }
                    }
                }
                //Calculate the simple curve on multiple threads (they dont't need the UnityAPI)
                var multiThreadTasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
                var mainThreadTasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
                for (int i = 0; i < boxes.Length; i++)
                {
                    if (boxes[i] != null && curves[i] != null)
                    {
                        Vector3 start = curves[i].start.transform.position;
                        Vector3 goal = curves[i].goal.transform.position;
                        BoundingBoxes box = boxes[i];
                        ConnectionCurve curve = curves[i];
                        multiThreadTasks.Add(Task.Run<Vector3[]>(() => { return SimpleCurveGerneration.StartGeneration(start, goal, box, 60); }),curve);
                    }
                }

                while (multiThreadTasks.Count > 0)
                {
                    Task<Vector3[]> finishedTask = await Task.WhenAny(multiThreadTasks.Keys);
                    ConnectionCurve connectionCurve = multiThreadTasks[finishedTask];
                    Vector3[] curve = finishedTask.Result;
                    //connectionCurve can somehow be null here
                    if (connectionCurve != null && !CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal))
                    {
                        connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
                        connectionCurve?.lineRenderer.SetPositions(finishedTask.Result);
                    }
                    else
                    {
                        mainThreadTasks.Add(JoinedCurve(connectionCurve, stepSize), connectionCurve);
                    }
                    multiThreadTasks.Remove(finishedTask);
                }

                //When necassary, calculate the pathfinding curves, but on the main thread (they need the UnityAPI, which is not thread safe)
                while (mainThreadTasks.Count > 0)
                {
                    Task<Vector3[]> finishedTask = await Task.WhenAny(mainThreadTasks.Keys);
                    ConnectionCurve connectionCurve = mainThreadTasks[finishedTask];
                    //connectionCurve can somehow be null here
                    if (connectionCurve != null)
                    {
                        connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
                        connectionCurve?.lineRenderer.SetPositions(finishedTask.Result);
                    }
                    mainThreadTasks.Remove(finishedTask);
                }
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.InnerException);
            throw e;
        }
    }

    static async Task<Vector3[]> JoinedCurve(ConnectionCurve connectionCurve, float stepSize, int segmentCount = 60)
    {
        try
        {
            Vector3[] curve;
            IntTriple startCell = IntTriple.VectorToCell(connectionCurve.start.transform.position, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(connectionCurve.goal.transform.position, stepSize);
            Task<GridSearch.SearchResult<IntTriple>> astarTask = AStar.AStarGridSearchAsync(startCell, goalCell, stepSize, connectionCurve.start, connectionCurve.goal);
            await astarTask;
            if (astarTask.Result.path == null)
            {
                Task<GridSearch.SearchResult<IntTriple>> greedyTask = Greedy.GreedyGridSearchAsync(startCell, goalCell, stepSize, connectionCurve.start, connectionCurve.goal);
                await greedyTask;
                if (greedyTask.Result.path == null)
                {
                    curve = SimpleCurveGerneration.StandartCurve(connectionCurve.start.transform.position, connectionCurve.goal.transform.position, segmentCount, 0.5f);
                }
                else
                {
                    curve = CurveGenerator.IntTripleArrayToCurve(greedyTask.Result.path, connectionCurve.start.transform.position, connectionCurve.goal.transform.position, stepSize);
                }
            }
            else
            {
                curve = CurveGenerator.IntTripleArrayToCurve(astarTask.Result.path, connectionCurve.start.transform.position, connectionCurve.goal.transform.position, stepSize);
            }
            
            return curve;
        }
        catch (Exception e)
        {
            Debug.LogError(e.InnerException);
            throw e;
        }
    }
}
