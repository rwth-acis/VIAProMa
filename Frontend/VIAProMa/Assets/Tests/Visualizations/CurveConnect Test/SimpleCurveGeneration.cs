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

        int witdhRes = 5;

        float distance = Vector3.Distance(startAdjusted, goalAdjusted);

        for (int heigthOffset = 3; heigthOffset <= 20; heigthOffset++)
        {

            for (int widthOffset = 0; widthOffset <= witdhRes; widthOffset++)
            {
                float widthOffsetMult = 0.4f + 0.6f * ((float)widthOffset/witdhRes);
                Vector3 cp2 = startAdjusted + new Vector3(0, heigthOffset, 0) + direction * distance * (1-widthOffsetMult);
                Vector3 cp3 = startAdjusted + new Vector3(0, heigthOffset, 0) + direction * distance * widthOffsetMult;

                Vector3[] checkCurve = BezierCurve.calculateCubicCurve(start, cp2, cp3, goal, 15);

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
}
