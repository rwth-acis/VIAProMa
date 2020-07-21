using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinedCurveGeneration
{
    public static Vector3[] start(Vector3 start, Vector3 goal, GameObject startBound, GameObject endBound, float stepSize)
    {
        Vector3[] curve = SimpleCurveGerneration.startContinous(start, goal, startBound, endBound);
        if (Curve.CurveCollsionCheck(curve, startBound, endBound))
        {
            List<IntTriple> path;
            IntTriple startCell = IntTriple.VectorToCell(start, stepSize);
            IntTriple goalCell = IntTriple.VectorToCell(goal, stepSize);
            if (Vector3.Distance(start, goal) < 18)
                path = AStar.AStarSearch<IntTriple>(startCell, goalCell, LineControllScriptFrameShare.GetNeighborsGenerator(stepSize), (x, y) => x == y, LineControllScriptFrameShare.HeuristicGenerator(goal, stepSize), LineControllScriptFrameShare.CostsBetweenGenerator(stepSize)).path;
            else
                path = Greedy.GreedySearch<IntTriple>(startCell, goalCell, LineControllScriptFrameShare.GetNeighborsGenerator(stepSize), (x, y) => x == y, LineControllScriptFrameShare.HeuristicGenerator(goal, stepSize), LineControllScriptFrameShare.CostsBetweenGenerator(stepSize)).path;
            curve = Curve.IntTripleArrayToCurve(path,start,goal,stepSize);
        }

        return curve;
    }
}
