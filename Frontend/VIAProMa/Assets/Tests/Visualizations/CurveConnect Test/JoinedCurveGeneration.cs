using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedCurveGeneration
{
    public static Vector3[] start(Vector3 start, Vector3 goal, GameObject startBound, GameObject endBound, float stepSize, int CurveSegmentCount = 60)
    {
        Vector3[] curve = SimpleCurveGerneration.StartGeneration(start, goal, startBound, endBound, CurveSegmentCount);
        if (CurveGenerator.CurveCollsionCheck(curve, startBound, endBound))
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
