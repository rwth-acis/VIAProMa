using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedCurveGeneration
{
    public static Vector3[] start(GameObject startObject, GameObject goalObject, float stepSize, int CurveSegmentCount = 60)
    {
        Vector3 start = startObject.transform.position;
        Vector3 goal = goalObject.transform.position;

        Vector3[] curve = SimpleCurveGerneration.StartGeneration(startObject, goalObject, CurveSegmentCount);
        if (CurveGenerator.CurveCollsionCheck(curve, startObject, goalObject))
        {
            List<IntTriple> path;
            IntTriple startCell = IntTriple.VectorToCell(start, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(goal, stepSize);
            if (Vector3.Distance(start, goal) < 18)
                path = AStar.AStarGridSearch(startCell, goalCell, stepSize, goal).path;
            else
                path = Greedy.GreedyGridSearch(startCell, goalCell, stepSize, goal).path;
            curve = CurveGenerator.IntTripleArrayToCurve(path,start,goal,stepSize);
        }

        return curve;
    }
}
