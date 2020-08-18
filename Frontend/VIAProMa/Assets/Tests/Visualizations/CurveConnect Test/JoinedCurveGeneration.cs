using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class JoinedCurveGeneration : MonoBehaviour
{
    public float standartHeight = 0.2f;
    public void UpdateCurve(ConnectionCurve connectioCurve, float stepSize, int CurveSegmentCount = 60)
    {
        Vector3 start = connectioCurve.start.transform.position;
        Vector3 goal = connectioCurve.goal.transform.position;

        Vector3[] curve = SimpleCurveGerneration.StartGeneration(connectioCurve.start, connectioCurve.goal, CurveSegmentCount);
        if (CurveGenerator.CurveCollsionCheck(curve, connectioCurve.start, connectioCurve.goal))
        {
            List<IntTriple> path;
            IntTriple startCell = IntTriple.VectorToCell(start, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(goal, stepSize);

            if (Vector3.Distance(start, goal) < 20)
            {
                if (connectioCurve.coroutineData.status == CoroutineStatus.NoHandler)
                    connectioCurve.coroutine = StartCoroutine(AStarHandler(startCell, goalCell, stepSize, connectioCurve));
            }
            else
            {
                path = Greedy.GreedyGridSearch(startCell, goalCell, stepSize, goal, connectioCurve.start, connectioCurve.goal).path;
                curve = CurveGenerator.IntTripleArrayToCurve(path, start, goal, stepSize);
                if (connectioCurve.coroutine != null)
                {
                    StopCoroutine(connectioCurve.coroutine);
                }
                connectioCurve.coroutineData = new CoroutineData();
                connectioCurve.lineRenderer.positionCount = curve.Length;
                connectioCurve.lineRenderer.SetPositions(curve);
            }
        }
        else
        {
            if (connectioCurve.coroutine != null)
            {
                StopCoroutine(connectioCurve.coroutine);
            }
            connectioCurve.coroutineData = new CoroutineData();
            connectioCurve.lineRenderer.positionCount = curve.Length;
            connectioCurve.lineRenderer.SetPositions(curve);
        }  
    }

    IEnumerator UpdateCurveCoroutine(ConnectionCurve connectionCurve, float stepSize, int curveSegmentCount = 60)
    {
        Vector3 start = connectionCurve.start.transform.position;
        Vector3 goal = connectionCurve.goal.transform.position;

        Vector3[] curve = SimpleCurveGerneration.StartGeneration(connectionCurve.start, connectionCurve.goal, curveSegmentCount);
        if (CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal))
        {
            IntTriple startCell = IntTriple.VectorToCell(start, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(goal, stepSize);
            CoroutineData data = new CoroutineData();
            yield return StartCoroutine(AStar.AStarGridSearchCoroutine(startCell, goalCell, stepSize, connectionCurve.start, connectionCurve.goal, data));
            if (data.status == CoroutineStatus.Failure)
            {
                data = new CoroutineData();
                if (data.status == CoroutineStatus.Failure)
                    curve = SimpleCurveGerneration.StandartCurve(connectionCurve.start.transform.position, connectionCurve.goal.transform.position, curveSegmentCount, 0.5f);
                else
                    curve = CurveGenerator.IntTripleArrayToCurve(data.output.path, start, goal, stepSize);
            }
            else
                curve = CurveGenerator.IntTripleArrayToCurve(data.output.path, start, goal, stepSize); ;

        }

        connectionCurve.lineRenderer.positionCount = curve.Length;
        connectionCurve.lineRenderer.SetPositions(curve);
    }

    public IEnumerator UpdaterCoroutine(List<ConnectionCurve> curves, ConnectionCurveWrapper tempCurve, float stepSize)
    {
        DateTime timeStamp;
        while (true)
        {
            timeStamp = DateTime.Now;
            foreach (ConnectionCurve curve in curves)
            {
                
                UpdateCurveCoroutine(curve,stepSize);
                if ((DateTime.Now - timeStamp).TotalMilliseconds > 10)
                {
                    Debug.Log("Next frame for curve generation");
                    yield return null;
                    timeStamp = DateTime.Now;
                }
                    
            }
            if (tempCurve.curve != null)
            {
                UpdateCurveCoroutine(tempCurve.curve, stepSize);
            }
            yield return null;
        }
    }

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

    IEnumerator AStarHandler(IntTriple startCell, IntTriple goalCell, float stepSize, ConnectionCurve connectionCurve)
    {
        CoroutineData data = new CoroutineData();
        data.status = CoroutineStatus.Running;
        connectionCurve.coroutineData = data;
        yield return StartCoroutine(AStar.AStarGridSearchCoroutine(startCell,goalCell,stepSize, connectionCurve.start, connectionCurve.goal, data));
        Vector3 start = connectionCurve.start.transform.position;
        Vector3 goal = connectionCurve.goal.transform.position;
        Vector3[] curve;
        List<IntTriple> path = new List<IntTriple>();
        switch (data.status)
        {
            case CoroutineStatus.Finished:
                path = connectionCurve.coroutineData.output.path;
                break;
            case CoroutineStatus.Failure:
                path = Greedy.GreedyGridSearch(startCell,goalCell,stepSize,goal, connectionCurve.goal, connectionCurve.start).path;
                break;
        }
        curve = CurveGenerator.IntTripleArrayToCurve(path, start, goal, stepSize);
        connectionCurve.lineRenderer.positionCount = curve.Length;
        connectionCurve.lineRenderer.SetPositions(curve);
        connectionCurve.coroutineData = new CoroutineData();
    }
}
