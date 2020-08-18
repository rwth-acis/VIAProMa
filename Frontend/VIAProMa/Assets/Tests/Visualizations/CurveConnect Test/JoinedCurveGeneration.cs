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
        while (true)
        {
            var tasks = new Dictionary<Task<Vector3[]>, ConnectionCurve>();
            foreach (ConnectionCurve connectionCurve in curves)
            {    
                tasks.Add(JoinedCurve(connectionCurve,stepSize),connectionCurve);
            }
            while (tasks.Count > 0)
            {
                Task<Vector3[]> finishedTask = await Task.WhenAny(tasks.Keys);
                ConnectionCurve connectionCurve = tasks[finishedTask];
                connectionCurve.lineRenderer.positionCount = finishedTask.Result.Length;
                connectionCurve.lineRenderer.SetPositions(finishedTask.Result); 
                tasks.Remove(finishedTask);
            }
            await Task.Yield();
        }
    }

    static async Task<Vector3[]> JoinedCurve(ConnectionCurve connectionCurve, float stepSize, int segmentCount = 60)
    {
        Vector3[] curve = SimpleCurveGerneration.StartGeneration(connectionCurve.start, connectionCurve.goal, segmentCount);
        if (CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal))
        {
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
                curve = CurveGenerator.IntTripleArrayToCurve(astarTask.Result.path,connectionCurve.start.transform.position, connectionCurve.goal.transform.position,stepSize);
            }
        }
        return curve;
    }
}
