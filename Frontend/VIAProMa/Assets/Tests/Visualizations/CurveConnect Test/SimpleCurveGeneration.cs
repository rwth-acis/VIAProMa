using System;
using UnityEngine;

public class SimpleCurveGerneration
{
    public static Vector3[] start(Vector3 start, Vector3 goal)
    {
        //Check for colliosons above the objects

        Vector3[] curve;

        Vector3 higherOne = start.y > goal.y ? start : goal;

        Vector3 startAdjusted = new Vector3(start.x, higherOne.y, start.z);
        Vector3 goalAdjusted = new Vector3(goal.x, higherOne.y, goal.z);

        Vector3 direction = goalAdjusted - startAdjusted;
        direction.Normalize();

        int witdhRes = 8;

        float distance = Vector3.Distance(startAdjusted, goalAdjusted);

        for (float heigthOffset = 3; heigthOffset <= 20; heigthOffset+=0.5f)
        {

            for (int widthOffset = 0; widthOffset <= witdhRes; widthOffset++)
            {
                float widthOffsetMult = 0.4f + 0.6f * ((float)widthOffset/witdhRes);
                Vector3 cp2 = startAdjusted + new Vector3(0, heigthOffset, 0) + direction * distance * (1-widthOffsetMult);
                Vector3 cp3 = startAdjusted + new Vector3(0, heigthOffset, 0) + direction * distance * widthOffsetMult;

                Vector3[] checkCurve = BezierCurve.calculateCubicCurve(start, cp2, cp3, goal, 10);

                //collision check
                bool collsion = false;
                for (int i = 0; i < checkCurve.Length - 2 && !collsion; i++)
                {
                    Vector3 checkDirection = checkCurve[i + 1] - checkCurve[i];
                    float checkLength = Vector3.Distance(checkCurve[i], checkCurve[i + 1]);
                    checkDirection.Normalize();

                    Vector3 center = checkCurve[i] + checkDirection * checkLength / 2;

                    if (LineControllScriptFrameShare.collisonWithObstacle(center, new Vector3(1, 1, checkLength), Quaternion.LookRotation(direction, new Vector3(0, 1, 0))))
                    {
                        collsion = true;
                    }
                }

                if (!collsion)
                {
                    return BezierCurve.calculateCubicCurve(start, cp2, cp3, goal, 50);
                } 
            }
        }
        return null;
    }

    public static Vector3[] startContinous(Vector3 start, Vector3 goal, GameObject[] pointVis)
    {
        //Check for colliosons above the objects
        float lowerOne = start.y < goal.y ? start.y : goal.y;

        Vector3 startAdjusted = new Vector3(start.x, lowerOne, start.z);
        Vector3 goalAdjusted = new Vector3(goal.x, lowerOne, goal.z);

        Vector3 direction = goalAdjusted - startAdjusted;
        direction.Normalize();

        float distance = Vector3.Distance(startAdjusted,goalAdjusted);

        Vector3 center = startAdjusted + direction * distance/2;

        //Generate bounding box:
        Vector3 minPoint = center + new Vector3(0,2,0) - direction*(distance*0.2f);
        Vector3 maxPoint = center + new Vector3(0, 2, 0) + direction * (distance * 0.2f);

        Collider[] colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center,new Vector3(0.2f,Math.Abs(start.y-goal.y)+20,distance/2.1f),Quaternion.LookRotation(direction,Vector3.up));

        foreach (Collider collider in colliders)
        {
            minPoint = new Vector3(Mathf.Min(minPoint.x,collider.bounds.min.x), Mathf.Min(minPoint.y, collider.bounds.min.y), Mathf.Min(minPoint.z, collider.bounds.min.z));
            maxPoint = new Vector3(Mathf.Max(maxPoint.x, collider.bounds.max.x), Mathf.Max(maxPoint.y, collider.bounds.max.y), Mathf.Max(maxPoint.z, collider.bounds.max.z));
        }

        Vector3[] controllPoints = new Vector3[6];
        controllPoints[0] = start;
        controllPoints[5] = goal;

        minPoint = new Vector3(minPoint.x, maxPoint.y, minPoint.z);

      

        Vector3 startOnMaxHeight = new Vector3(start.x, maxPoint.y, start.z);

        //calculate the potential intersections from the line spanned by startOnMaxHeight+l*direction with the 4 lines that form the rectangle spanned by min and max
        Vector3[] potentialPoints = new Vector3[4];

        potentialPoints[0] = startOnMaxHeight + (minPoint.z - start.z) / direction.z * direction;
        potentialPoints[1] = startOnMaxHeight + (minPoint.x - start.x) / direction.x * direction;

        potentialPoints[2] = startOnMaxHeight + (maxPoint.z - start.z) / direction.z * direction;
        potentialPoints[3] = startOnMaxHeight + (maxPoint.x - start.x) / direction.x * direction;

        Vector3[] intersectionPoints = new Vector3[2];
        intersectionPoints[0] = new Vector3(float.NaN, float.NaN, float.NaN);
        intersectionPoints[1] = new Vector3(float.NaN, float.NaN, float.NaN);

        foreach (Vector3 point in potentialPoints)
        {
            //Is the vec not NaN and inside the rectangle?
            if (!float.IsNaN(point.x) && VecGreaterEqVec(point, minPoint) && VecSmallerEqVec(point, maxPoint))
            {
                intersectionPoints[float.IsNaN(intersectionPoints[0].x) ? 0 : 1] = point;
            }
        }

        //cp1 has to be the one that is closer to start
        if (Vector3.Distance(start, intersectionPoints[0]) < Vector3.Distance(start, intersectionPoints[1]))
        {
            controllPoints[1] = intersectionPoints[0];
            controllPoints[4] = intersectionPoints[1];
        }
        else
        {
            controllPoints[1] = intersectionPoints[1];
            controllPoints[4] = intersectionPoints[0];
        }

        controllPoints[1] = controllPoints[1] + new Vector3(0, 1+(maxPoint.y-startAdjusted.y)*0.2f, 0) - direction;
        controllPoints[4] = controllPoints[4] + new Vector3(0, 1 + (maxPoint.y - startAdjusted.y) * 0.2f, 0) + direction;

        controllPoints[2] = controllPoints[1] + new Vector3(0, 2, 0) - 2*direction;
        controllPoints[3] = controllPoints[4] + new Vector3(0, 2, 0) + 2*direction;

        pointVis[1].transform.position = controllPoints[1];
        pointVis[2].transform.position = controllPoints[2];
        pointVis[3].transform.position = controllPoints[3];
        pointVis[4].transform.position = controllPoints[4];

        return BezierCurve.calculateCurve(controllPoints,50);
    }


    static bool VecSmallerEqVec(Vector3 vec1, Vector3 vec2)
    {
        return (vec1.x < vec2.x || FloatEq(vec1.x,vec2.x)) && (vec1.y < vec2.y || FloatEq(vec1.y, vec2.y)) && (vec1.z < vec2.z || FloatEq(vec1.z, vec2.z));
    }

    static bool VecGreaterEqVec(Vector3 vec1, Vector3 vec2)
    {
        return (vec1.x > vec2.x || FloatEq(vec1.x, vec2.x)) && (vec1.y > vec2.y || FloatEq(vec1.y, vec2.y)) && (vec1.z > vec2.z || FloatEq(vec1.z, vec2.z));
    }

    static bool FloatEq(float x, float y)
    {
        return Math.Abs(x - y) < 0.001f;
    }
}
