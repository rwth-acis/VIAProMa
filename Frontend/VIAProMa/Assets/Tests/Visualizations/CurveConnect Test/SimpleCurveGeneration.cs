using System;
using System.Collections.Generic;
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

    static bool CurveCollsionCheck(Vector3[] curve)
    {
        for (int i = 0; i < curve.Length - 2; i++)
        {
            Vector3 checkDirection = curve[i + 1] - curve[i];
            float checkLength = Vector3.Distance(curve[i], curve[i + 1]);
            checkDirection.Normalize();

            Vector3 center = curve[i] + checkDirection * checkLength / 2;

            if (LineControllScriptFrameShare.collisonWithObstacle(center, new Vector3(1, 1, checkLength), Quaternion.LookRotation(checkDirection, new Vector3(0, 1, 0))))
            {
                return true;
            }
        }
        return false;
    }

    enum Direction
    {
        Left,
        Right
    }

    public static Vector3[] startContinous(Vector3 start, Vector3 goal, GameObject[] pointVis)
    {
        

        
        float higherOne = start.y > goal.y ? start.y : goal.y;

        Vector3 startAdjusted = new Vector3(start.x, higherOne, start.z);
        Vector3 goalAdjusted = new Vector3(goal.x, higherOne, goal.z);

        Vector3 direction = goalAdjusted - startAdjusted;
        direction.Normalize();

        float distance = Vector3.Distance(startAdjusted,goalAdjusted);

        Vector3 center = startAdjusted + direction * distance/2;

        float standartHeight = 4;

       

  

        //Priority 1: Try to draw the standart curve
        Vector3[] checkCurve = BezierCurve.calculateCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, 4, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, 4, 0), goal }, 15);
        if (!CurveCollsionCheck(checkCurve))
        {
            return BezierCurve.calculateCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, 4, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, 4, 0), goal }, 50);
        }


        //Genenerating the standart curve didn't work due to collsions. Try to set the controllpoints so, that no collion occures

        //Generate bounding box:
        Vector3 minPoint = center; //+ new Vector3(0,2,0) - direction*(distance*0.2f);
        Vector3 maxPoint = center; //+ new Vector3(0, 2, 0) + direction * (distance * 0.2f);

        Collider[] colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center,new Vector3(0.2f,Math.Abs(start.y-goal.y)+20,distance/2.1f),Quaternion.LookRotation(direction,Vector3.up));

        foreach (Collider collider in colliders)
        {
            minPoint = new Vector3(Mathf.Min(minPoint.x,collider.bounds.min.x), Mathf.Min(minPoint.y, collider.bounds.min.y), Mathf.Min(minPoint.z, collider.bounds.min.z));
            maxPoint = new Vector3(Mathf.Max(maxPoint.x, collider.bounds.max.x), Mathf.Max(maxPoint.y, collider.bounds.max.y), Mathf.Max(maxPoint.z, collider.bounds.max.z));
        }


        minPoint = new Vector3(minPoint.x, maxPoint.y, minPoint.z);

        Vector3 boundingBoxCenter = minPoint + (maxPoint - minPoint) * 0.5f;

        Vector3[] intersectionPointsAbove = calculateIntersectionAbove(start,goal,minPoint,maxPoint,direction);
        Vector3[] intersectionPointsSide = calculateIntersectionSide(start,goal,minPoint,maxPoint,direction,standartHeight);


        float distanceAbove = LineControllScriptFrameShare.pathLength(new Vector3[] { start, intersectionPointsAbove[0], intersectionPointsAbove[1], goal});
        float distanceSide;
        if (intersectionPointsSide.Length == 1)
            distanceSide = LineControllScriptFrameShare.pathLength(new Vector3[] { start, intersectionPointsSide[0], goal });
        else
            distanceSide = LineControllScriptFrameShare.pathLength(new Vector3[] { start, intersectionPointsSide[0], intersectionPointsSide[1], goal });

        Vector3[] controllPoints = new Vector3[0];

        if (distanceAbove < 1.5 * distanceSide)
        {
            /*
            controllPoints = new Vector3[6];
            controllPoints[0] = start;
            controllPoints[5] = goal;
            //Shift them away from the obstacle to prevent collisions
            controllPoints[1] = intersectionPointsAbove[0] + new Vector3(0, 1 + (maxPoint.y - startAdjusted.y) * 0.2f, 0) - direction * (distance / 2) / (5 + Vector3.Distance(start, new Vector3(controllPoints[1].x, start.y, controllPoints[1].z)));
            controllPoints[4] = intersectionPointsAbove[1] + new Vector3(0, 1 + (maxPoint.y - startAdjusted.y) * 0.2f, 0) + direction * (distance / 2) / (5 + Vector3.Distance(goal, new Vector3(controllPoints[4].x, start.y, controllPoints[4].z)));

            controllPoints[2] = controllPoints[1] + new Vector3(0, 2, 0) - 2 * direction;
            controllPoints[3] = controllPoints[4] + new Vector3(0, 2, 0) + 2 * direction;

            //pointVis[1].transform.position = controllPoints[1];
            //pointVis[2].transform.position = controllPoints[2];
            //pointVis[3].transform.position = controllPoints[3];
            //pointVis[4].transform.position = controllPoints[4];
            */

            Vector3[] controllPointsCurve1 = new Vector3[3];
            Vector3[] controllPointsCurve2 = new Vector3[4];
            Vector3[] controllPointsCurve3 = new Vector3[3];

            //Curve 1
            controllPointsCurve1[0] = start;
            controllPointsCurve1[2] = intersectionPointsAbove[0];
            //cp Lies in the centroid of the trianlgle spanned by cPC1[0],cPC1[2] and the point (cPC1[0].x, cPC1[2].y, cPC1[0].x.z)
            controllPointsCurve1[1] = CalculateCentroid(controllPointsCurve1[0], controllPointsCurve1[2]);

            //Curve 3
            controllPointsCurve3[0] = intersectionPointsAbove[1];
            controllPointsCurve3[2] = goal;
            controllPointsCurve3[1] = CalculateCentroid(controllPointsCurve3[2], controllPointsCurve3[0]);

            //Curve 2 that connects Curve 1 and 3
            controllPointsCurve2[0] = controllPointsCurve1[2];
            controllPointsCurve2[3] = controllPointsCurve3[0];

            Vector3 aboveMiddle = CalculateLineIntersection(controllPointsCurve1[1], controllPointsCurve1[2] - controllPointsCurve1[1], controllPointsCurve3[1], controllPointsCurve3[0] - controllPointsCurve3[1]);
            controllPointsCurve2[1] = controllPointsCurve2[0] + (aboveMiddle - controllPointsCurve2[0]) / 2;
            controllPointsCurve2[2] = controllPointsCurve2[3] + (aboveMiddle - controllPointsCurve2[3]) / 2;

            Vector3[] curve1 = BezierCurve.calculateCurve(controllPointsCurve1, 20);
            Vector3[] curve2 = BezierCurve.calculateCurve(controllPointsCurve2, 20);
            Vector3[] curve3 = BezierCurve.calculateCurve(controllPointsCurve3, 20);
            Vector3[] curve = new Vector3[curve1.Length + curve2.Length + curve3.Length];
            curve1.CopyTo(curve,0);
            curve2.CopyTo(curve, curve1.Length);
            curve3.CopyTo(curve, curve1.Length+ curve2.Length);

            return curve;
        }

        else
        {
            Vector3 boundingBoxCenterShifted = new Vector3(boundingBoxCenter.x, standartHeight, boundingBoxCenter.z);
            if (intersectionPointsSide.Length == 1)
            {
                controllPoints = new Vector3[3];
                controllPoints[0] = start;
                controllPoints[1] = shiftAway(intersectionPointsSide[0], boundingBoxCenterShifted);
                controllPoints[2] = goal;
            }
            else
            {
                controllPoints = new Vector3[4];
                controllPoints[0] = start;
                controllPoints[1] = shiftAway(intersectionPointsSide[0], boundingBoxCenterShifted);
                controllPoints[2] = shiftAway(intersectionPointsSide[1], boundingBoxCenterShifted);
                controllPoints[3] = goal;
            }

        }

        return BezierCurve.calculateCurve(controllPoints, 50);

    }

    static Vector3[] calculateIntersectionAbove(Vector3 start, Vector3 goal, Vector3 minPoint, Vector3 maxPoint, Vector3 direction)
    {
        Vector3 startOnMaxHeight = new Vector3(start.x, maxPoint.y, start.z);
        //Calculate the controll points for the way above
        //calculate the potential intersections from the line spanned by startOnMaxHeight+l*direction with the 4 lines that form the rectangle spanned by min and max
        Vector3[] potentialPoints = new Vector3[4];

        potentialPoints[0] = startOnMaxHeight + (minPoint.z - start.z) / direction.z * direction;
        potentialPoints[1] = startOnMaxHeight + (minPoint.x - start.x) / direction.x * direction;

        potentialPoints[2] = startOnMaxHeight + (maxPoint.z - start.z) / direction.z * direction;
        potentialPoints[3] = startOnMaxHeight + (maxPoint.x - start.x) / direction.x * direction;



        bool differentValue = true;

        //Check if there is more then one valid intersection
        foreach (Vector3 point in potentialPoints)
        {
            Vector3 firstReal = new Vector3(float.NaN, float.NaN, float.NaN);

            if (!float.IsNaN(point.x))
            {
                if (!float.IsNaN(firstReal.x))
                {
                    firstReal = point;
                }
                else
                {
                    differentValue &= VecEqVec(point, firstReal);
                }
            }
        }

        Vector3[] intersectionPointsAbove = new Vector3[2];
        intersectionPointsAbove[0] = new Vector3(float.NaN, float.NaN, float.NaN);
        intersectionPointsAbove[1] = new Vector3(float.NaN, float.NaN, float.NaN);

        foreach (Vector3 point in potentialPoints)
        {
            //Is the vec not NaN and inside the rectangle?
            if (!float.IsNaN(point.x) && VecGreaterEqVec(point, minPoint) && VecSmallerEqVec(point, maxPoint))
            {
                if (float.IsNaN(intersectionPointsAbove[0].x))
                {
                    intersectionPointsAbove[0] = point;
                }
                else if (!differentValue || !VecEqVec(point, intersectionPointsAbove[0]))
                {
                    intersectionPointsAbove[1] = point;
                }
            }
        }



        //cp1 has to be the one that is closer to start
        if (Vector3.Distance(start, intersectionPointsAbove[0]) > Vector3.Distance(start, intersectionPointsAbove[1]))
        {
            Vector3 temp = intersectionPointsAbove[0];
            intersectionPointsAbove[0] = intersectionPointsAbove[1];
            intersectionPointsAbove[1] = temp;
        }

        return intersectionPointsAbove;
    }
    static Vector3[] calculateIntersectionSide(Vector3 start, Vector3 goal, Vector3 minPoint, Vector3 maxPoint, Vector3 direction, float standartHeight)
    {
        //Calculate the controll points at the left/right

        //Determine the collsion points for left/right
        Vector3 leftLower = new Vector3(minPoint.x, standartHeight, minPoint.z);
        Vector3 leftUpper = new Vector3(minPoint.x, standartHeight, maxPoint.z);
        Vector3 rightUpper = new Vector3(maxPoint.x, standartHeight, maxPoint.z);
        Vector3 rightLower = new Vector3(maxPoint.x, standartHeight, minPoint.z);

        Vector3[] edgePoints = new Vector3[] { leftLower, leftUpper, rightUpper, rightLower };



        Dictionary<bool, List<Vector3>> edgesSorted = new Dictionary<bool, List<Vector3>>(); //edgesSorted[false] yields all edges left from the line between start and goal and edgesSorted[true] all right
        edgesSorted.Add(false, new List<Vector3>());
        edgesSorted.Add(true, new List<Vector3>());

        List<Vector3> intersectionPointsSide = new List<Vector3>();

        float impliciteLineFromStartToGoal(Vector3 point)
        {
            Vector2 normal = new Vector3(direction.z, -direction.x);
            return (normal.x * point.x + normal.y * point.z) - (normal.x * start.x + normal.y * start.z);
        }


        foreach (Vector3 edge in edgePoints)
        {
            edgesSorted[impliciteLineFromStartToGoal(edge) < 0].Add(edge);
        }

        edgesSorted[false].Sort((x, y) => { return (int)(Vector3.Distance(x, start) - Vector3.Distance(y, start)); });
        edgesSorted[true].Sort((x, y) => { return (int)(Vector3.Distance(x, start) - Vector3.Distance(y, start)); });

        if (edgesSorted[false].Count == edgesSorted[true].Count)
        {
            //Determine which pair has the smaller way
            bool closerPair;

            float approximateWayLeft = Vector3.Distance(start, edgesSorted[false][0]) + Vector3.Distance(goal, edgesSorted[false][1]);
            float approximateWayRight = Vector3.Distance(start, edgesSorted[true][0]) + Vector3.Distance(goal, edgesSorted[true][1]);

            closerPair = approximateWayLeft > approximateWayRight;


            intersectionPointsSide.Add(edgesSorted[closerPair][0]);
            intersectionPointsSide.Add(edgesSorted[closerPair][1]);
        }
        else
        {
            if (edgesSorted[false].Count == 1)
            {
                intersectionPointsSide.Add(edgesSorted[false][0]);
            }
            else if (edgesSorted[true].Count == 1)
            {
                intersectionPointsSide.Add(edgesSorted[true][0]);
            }

        }

        return intersectionPointsSide.ToArray();
    }

    static Vector3 shiftAway(Vector3 vec, Vector3 center)
    {
        return vec + (vec - center) * 0.5f;
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
        return Math.Abs(x - y) < 0.1f;
    }

    static bool VecEqVec(Vector3 vec1, Vector3 vec2)
    {
        return FloatEq(vec1.x, vec2.x) && FloatEq(vec1.y, vec2.y) && FloatEq(vec1.z, vec2.z);
    }

    static Vector3 CalculateCentroid(Vector3 p0, Vector3 p2)
    {
        Vector3 p1 = new Vector3(p0.x, p2.y, p0.z);

        Vector3 p01 = p1 + (p0 - p1) / 2;
        Vector3 p12 = p2 + (p1 - p2) / 2;
        Vector3 d1 = p2 - p01;
        Vector3 d2 = p0 - p12;

        //float r = (p12.y - p01.y - ((d1.y * p12.x + p01.x) / (d1.x))) / (d2.x / d1.x - d2.y);

        //return p12 + r*d2;
        return CalculateLineIntersection(p01, d1, p12, d2);
    }

    static Vector3 CalculateLineIntersection(Vector3 startPoint1, Vector3 direction1, Vector3 startPoint2, Vector3 direction2)
    {
        //float r = (startPoint2.y - startPoint1.y - ((direction1.y * startPoint2.x + startPoint1.x) / (direction1.x))) / (direction2.x / direction1.x - direction2.y);
        float r = (startPoint2.y-startPoint1.y- (direction1.y*(startPoint2.x - startPoint1.x) / (direction1.x))) / ((direction1.y*direction2.x)/(direction1.x) - direction2.y);
        return startPoint2 + r * direction2;
    }
}
