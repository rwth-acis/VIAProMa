using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedCurveGeneration : MonoBehaviour
{
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
                if (connectioCurve.coroutineData.status == AStarStatus.NoHandler)
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
                connectioCurve.coroutineData = new AStarParameter();
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
            connectioCurve.coroutineData = new AStarParameter();
            connectioCurve.lineRenderer.positionCount = curve.Length;
            connectioCurve.lineRenderer.SetPositions(curve);
        }  
    }

    IEnumerator AStarHandler(IntTriple startCell, IntTriple goalCell, float stepSize, ConnectionCurve connectionCurve)
    {
        AStarParameter data = new AStarParameter(startCell, goalCell, stepSize, connectionCurve.goal.transform.position, connectionCurve.start, connectionCurve.goal);
        connectionCurve.coroutineData = data;
        yield return StartCoroutine(AStar.AStarSearchCoroutine(data));
        Vector3 start = connectionCurve.start.transform.position;
        Vector3 goal = connectionCurve.goal.transform.position;
        Vector3[] curve;
        List<IntTriple> path = new List<IntTriple>();
        switch (data.status)
        {
            case AStarStatus.Finished:
                path = connectionCurve.coroutineData.output.path;
                break;
            case AStarStatus.Failure:
                path = Greedy.GreedyGridSearch(startCell,goalCell,stepSize,goal, connectionCurve.goal, connectionCurve.start).path;
                break;
        }
        curve = CurveGenerator.IntTripleArrayToCurve(path, start, goal, stepSize);
        connectionCurve.lineRenderer.positionCount = curve.Length;
        connectionCurve.lineRenderer.SetPositions(curve);
        connectionCurve.coroutineData = new AStarParameter();
    }
}
