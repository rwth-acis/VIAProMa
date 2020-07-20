using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCurveGerneration
{
    public static float distanceToObstacle = 0.3f;
    public static Vector3[] start(Vector3 start, Vector3 goal)
    {
        //Check for colliosons above the objects

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

                Vector3[] checkCurve = Curve.CalculateBezierCurve( new Vector3[] { start, cp2, cp3, goal }, 10);

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
                    return Curve.CalculateBezierCurve(new Vector3[] { start, cp2, cp3, goal }, 50);
                } 
            }
        }
        return null;
    }

    static Vector3[] StandartCurve(Vector3 start, Vector3 goal, int segmentCount, float height)
    {
        float higherOne = start.y > goal.y ? start.y : goal.y;

        Vector3 startAdjusted = new Vector3(start.x, higherOne, start.z);
        Vector3 goalAdjusted = new Vector3(goal.x, higherOne, goal.z);
        Vector3 direction = goalAdjusted - startAdjusted;
        direction.Normalize();
        float distance = Vector3.Distance(startAdjusted, goalAdjusted);
        Vector3 center = startAdjusted + direction * distance / 2;

        return Curve.CalculateBezierCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, height, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, height, 0), goal }, 50);
    }

    public static Vector3[] startContinous(Vector3 start, Vector3 goal, GameObject startBound, GameObject endBound, GameObject[] pointVis = null)
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
        Vector3[] checkCurve = StandartCurve(start,goal,15,standartHeight);
        if (!Curve.CurveCollsionCheck(checkCurve))
        {
            return StandartCurve(start, goal, 50, standartHeight);
        }


        //Genenerating the standart curve didn't work due to collsions. Try to set the controllpoints so, that no collisions occure

        //Sets min and max in such a way that they include as many of the obstacles as possible but never start or goal
        void SetMinMax(Collider[] colliderss, ref Vector3 min, ref Vector3 max)
        {
            foreach (Collider collider in colliderss)
            {
                min = Vector3.Min(min, collider.bounds.min);
                max = Vector3.Max(max, collider.bounds.max);
                float correctionOffset = distance * 0.1f;

                if (VecGreaterEqVec(start, min) && VecSmallerEqVec(start, max))
                {                  
                    Vector3 startGoal = goal - start;
                    Vector2 correction = new Vector2(startGoal.x, startGoal.z);
                    correction.Normalize();
                    if (Mathf.Abs(correction.x) > Mathf.Abs(correction.y))
                        correction = new Vector2(Mathf.Sign(correction.x),0);
                    else
                        correction = new Vector2(0,Mathf.Sign(correction.y));
                    if(correction.x < 0)
                        max.x = start.x - correctionOffset;
                    else if(correction.y < 0)
                        max.z = start.z - correctionOffset;
                    else if(correction.x > 0)
                        min.x = start.x + correctionOffset;
                    else
                        min.z = start.z + correctionOffset;
                }
                if (VecGreaterEqVec(goal, min) && VecSmallerEqVec(goal, max))
                {
                    Vector3 goalStart = start - goal;
                    Vector2 correction = new Vector2(goalStart.x, goalStart.z);
                    correction.Normalize();
                    if (Mathf.Abs(correction.x) > Mathf.Abs(correction.y))
                        correction = new Vector2(Mathf.Sign(correction.x), 0);
                    else
                        correction = new Vector2(0, Mathf.Sign(correction.y));
                    if (correction.x < 0)
                        max.x = goal.x - correctionOffset;
                    else if (correction.y < 0)
                        max.z = goal.z - correctionOffset;
                    else if (correction.x > 0)
                        min.x = goal.x + correctionOffset;
                    else
                        min.z = goal.z + correctionOffset;
                }
            }
        }

        //Generate bounding box for above:
        Vector3 minPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        Collider[] colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center,new Vector3(0.2f,Math.Abs(start.y-goal.y)+20,distance/2.1f),Quaternion.LookRotation(direction,Vector3.up), startBound, endBound);

        SetMinMax(colliders, ref minPoint, ref maxPoint);

        if (minPoint == maxPoint)
        {
            minPoint += new Vector3(-1, -1, -1);
            maxPoint += new Vector3(1, 1, 1);
        }

        minPoint = new Vector3(minPoint.x, maxPoint.y, minPoint.z);

        //Generate bounding box for the sides:
        Vector3 minPointSide = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPointSide = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center + new Vector3(0,2,0), new Vector3(3, 4, distance / 2.1f), Quaternion.LookRotation(direction, Vector3.up), startBound, endBound);
        SetMinMax(colliders, ref minPointSide, ref maxPointSide);

        Vector3 reCenter = minPointSide + (maxPointSide - minPointSide) * 0.5f;
        Vector3 reExtend = maxPointSide - minPointSide + new Vector3(3, 0, 3);
        reExtend.y = standartHeight + 2;
        reExtend /= 2;
        reCenter.y = reExtend.y;
        
        Collider[] colllidersReScan = LineControllScriptFrameShare.GetCollidorsFromObstacles(reCenter, reExtend, Quaternion.identity, startBound, endBound);
        SetMinMax(colllidersReScan, ref minPointSide, ref maxPointSide);

        for (int i = 0; i < 5 && colliders.Length != colllidersReScan.Length; i++)
        {
            colliders = colllidersReScan;
            reCenter = minPointSide + (maxPointSide - minPointSide) * 0.5f;
            reExtend = maxPointSide - minPointSide + new Vector3(3, 0, 3);
            reExtend.y = standartHeight + 2;
            reExtend /= 2;
            reCenter.y = reExtend.y;
            colllidersReScan = LineControllScriptFrameShare.GetCollidorsFromObstacles(reCenter, reExtend, Quaternion.identity, startBound, endBound);
            SetMinMax(colllidersReScan, ref minPointSide, ref maxPointSide);
        }


        pointVis[0].transform.position = minPointSide;
        pointVis[1].transform.position = maxPointSide;

        Vector3[] intersectionPointsAbove = calculateIntersectionAbove(start,goal,minPoint,maxPoint,direction);
        Vector3[] intersectionPointsSide = calculateIntersectionSide(start,goal, minPointSide, maxPointSide, direction,standartHeight);



        float distanceAbove = Curve.CurveLength(new Vector3[] { start, intersectionPointsAbove[0], intersectionPointsAbove[1], goal});
        float distanceSide;
        if (intersectionPointsSide.Length == 1)
            distanceSide = Curve.CurveLength(new Vector3[] { start, intersectionPointsSide[0], goal });
        else if(intersectionPointsSide.Length == 2)
            distanceSide = Curve.CurveLength(new Vector3[] { start, intersectionPointsSide[0], intersectionPointsSide[1], goal });
        else
            return Curve.CalculateBezierCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, 4, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, 4, 0), goal }, 50);

        Vector3[] controllPoints = new Vector3[0];

        if (distanceAbove < 1.5 * distanceSide)
        {
            //return CalculateJoinedCurve(start, intersectionPointsAbove, goal, Vector3.up);
            return CalculateJoinedCurve(start, intersectionPointsAbove, goal, 15);
        }
        else
        {
            //return CalculateJoinedCurveSide(start, intersectionPointsSide, goal, pointVis[0], pointVis[1], pointVis[2], pointVis[3]);
            return CalculateJoinedCurve(start, intersectionPointsSide, goal, 15);
        }
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

        //Check if there is more than one valid intersection
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
        Vector2 CalculatePlacmentToBoundingbox(Vector3 point, Vector3 min, Vector3 max)
        {
            Vector2 pointVec2 = new Vector2(point.x, point.z);
            Vector2 minVec2 = new Vector2(min.x, min.z);
            Vector2 maxVec2 = new Vector2(max.x, max.z);

            Vector2 placment = new Vector2();
            if (pointVec2.x < minVec2.x)
                placment.x = 0;
            else
            {
                if (pointVec2.x < maxVec2.x)
                    placment.x = 1;
                else
                    placment.x = 2;
            }

            if (pointVec2.y < minVec2.y)
                placment.y = 0;
            else
            {
                if (pointVec2.y < maxVec2.y)
                    placment.y = 1;
                else
                    placment.y = 2;
            }
            return placment;
        }
        

        //Calculate the controll points at the left/right

        float higherY = Mathf.Max(start.y , goal.y);

        //Determine the collsion points for left/right
        Vector3[,] edgePoints = new Vector3[3,3];
        edgePoints[0,0] = new Vector3(minPoint.x, higherY + standartHeight, minPoint.z);
        edgePoints[0,2] = new Vector3(minPoint.x, higherY + standartHeight, maxPoint.z);
        edgePoints[2,0] = new Vector3(maxPoint.x, higherY + standartHeight, minPoint.z);
        edgePoints[2,2] = new Vector3(maxPoint.x, higherY + standartHeight, maxPoint.z);



        Dictionary<bool, List<Vector3>> edgesSorted = new Dictionary<bool, List<Vector3>>(); //edgesSorted[false] yields all edges left from the line between start and goal and edgesSorted[true] all right
        edgesSorted.Add(false, new List<Vector3>());
        edgesSorted.Add(true, new List<Vector3>());

        List<Vector3> intersectionPointsSide = new List<Vector3>();

        Vector2 startPlacment = CalculatePlacmentToBoundingbox(start, minPoint, maxPoint);
        Vector2 goalPlacment = CalculatePlacmentToBoundingbox(goal, minPoint, maxPoint);
        Vector2 currentPosition = startPlacment;

        Vector2 TryDirection(Vector2 curr, Vector2 dir)
        {
            curr += dir;
            if (curr.x < 0 || curr.x > 2 || curr.y < 0 || curr.y > 2 || (curr.x == 1 && curr.y == 1))
            {
                return new Vector2(float.NaN, float.NaN);
            }
            else
            {
                return curr;
            }
        }
        Vector2 NextDirection(Vector2 dir)
        {
            if (dir.x == -1)
                return new Vector2(0, 1);
            else if (dir.y == 1)
                return new Vector2(1, 0);
            else if (dir.x == 1)
                return new Vector2(0, -1);
            else
                return new Vector2(float.NaN,float.NaN);
        }

        //Is one of them inside the box?
        if (startPlacment == new Vector2(1, 1) || goalPlacment == new Vector2(1, 1))
            return new Vector3[0];

        Vector2 oldPosition = currentPosition;
        Vector2 nextPosition = currentPosition;
        for (Vector2 dir = new Vector2(-1, 0); !float.IsNaN(dir.x); dir = NextDirection(dir))
        {
            nextPosition = TryDirection(currentPosition, dir);
            if (!float.IsNaN(nextPosition.x))
            {
                currentPosition = nextPosition;
                //Path 1
                Vector2 oldDir = dir;
                Vector2 nextDir = new Vector2(-1, 0);
                while (currentPosition != goalPlacment)
                {
                    for (nextDir = new Vector2(-1, 0); !float.IsNaN(nextDir.x); nextDir = NextDirection(nextDir))
                    {
                        nextPosition = TryDirection(currentPosition,nextDir);
                        if (!float.IsNaN(nextPosition.x) && nextPosition != oldPosition)
                        {
                            oldPosition = currentPosition;
                            currentPosition = nextPosition;
                            if (oldDir != nextDir)
                            {
                                edgesSorted[false].Add(edgePoints[(int)oldPosition.x, (int)oldPosition.y]);
                            }
                            oldDir = nextDir;
                            break;
                        }
                    }
                }

                //Path 2
                //find new start direction
                
                for (nextDir = new Vector2(-1, 0); !float.IsNaN(nextDir.x); nextDir = NextDirection(nextDir))
                {
                    if (!float.IsNaN(TryDirection(startPlacment, nextDir).x) && nextDir != dir)
                    {
                        break;
                    } 
                }
                //go path 2
                currentPosition = TryDirection(startPlacment, nextDir);
                oldDir = nextDir;
                oldPosition = startPlacment;
                while (currentPosition != goalPlacment)
                {
                    for (nextDir = new Vector2(-1, 0); !float.IsNaN(nextDir.x); nextDir = NextDirection(nextDir))
                    {
                        nextPosition = TryDirection(currentPosition, nextDir);
                        if (!float.IsNaN(nextPosition.x) && nextPosition != oldPosition)
                        {
                            oldPosition = currentPosition;
                            currentPosition = nextPosition;
                            if (oldDir != nextDir)
                            {
                                edgesSorted[true].Add(edgePoints[(int)oldPosition.x, (int)oldPosition.y]);
                            }
                            oldDir = nextDir;
                            break;
                        }
                    }
                }
                break;
            }
        }

        if (edgesSorted[false].Count != edgesSorted[true].Count)
        {
            if (edgesSorted[false].Count < edgesSorted[true].Count)
                return edgesSorted[false].ToArray();
            else
                return edgesSorted[true].ToArray();
        }
        else
        {
            float approximateWayLeft = Vector3.Distance(start, edgesSorted[false][0]) + Vector3.Distance(goal, edgesSorted[false][1]);
            float approximateWayRight = Vector3.Distance(start, edgesSorted[true][0]) + Vector3.Distance(goal, edgesSorted[true][1]);

            return edgesSorted[approximateWayLeft > approximateWayRight].ToArray();
        }
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

    static Vector2 CalculateCentroid(Vector2 p0, Vector2 p1, Vector2 p2)
    {
        Vector2 p01 = p1 + (p0 - p1) / 2;
        Vector2 p12 = p2 + (p1 - p2) / 2;
        Vector2 d1 = p2 - p01;
        Vector2 d2 = p0 - p12;

        return CalculateLineIntersection(p01, d1, p12, d2);
    }

    static Vector2 CalculateLineIntersection(Vector2 startPoint1, Vector2 direction1, Vector2 startPoint2, Vector2 direction2)
    {
        float r = float.NaN;
        float l = float.NaN;
        if (direction1.x == 0 && direction2.x != 0)
        {
            r = (startPoint1.x - startPoint2.x) / direction2.x;
        }
        else if (direction1.y == 0 && direction2.y != 0)
        {
            r = (startPoint1.y - startPoint2.y) / direction2.y;
        }
        else if (direction2.x == 0 && direction1.x != 0)
        {
            l = (startPoint2.x - startPoint1.x) / direction1.x;
        }
        else if (direction2.y == 0 && direction1.y != 0)
        {
            l = (startPoint2.y - startPoint1.y) / direction1.y;
        }
        else if (direction1.x == 0 && direction2.x == 0 || direction1.y == 0 && direction2.y == 0)
        {
            return startPoint1;
        }
        else
        {
            r = (startPoint2.y - startPoint1.y - (direction1.y * (startPoint2.x - startPoint1.x) / (direction1.x))) / ((direction1.y * direction2.x) / (direction1.x) - direction2.y);
        }
        if (!float.IsNaN(r))
            return startPoint2 + r * direction2;
        else
            return startPoint1 + l * direction1;
    }

    public static Vector3[] CalculateJoinedCurve(Vector3 start, Vector3[] middlePoints, Vector3 goal, int segmentCount, GameObject g0 = null, GameObject g1 = null, GameObject g2 = null, GameObject g3 = null)
    {
        if (middlePoints.Length == 0)
        {
            return StandartCurve(start, goal, 50, 4);
        }

        Vector3[] controllPointsCurve1 = new Vector3[3];
        Vector3[] controllPointsCurve2 = new Vector3[4];
        Vector3[] controllPointsCurve3 = new Vector3[3];
        int higherOne = start.y > goal.y ? 0 : 1;

        Vector3 direction = goal - start;
        direction.Normalize();
        Vector3 normal;
        //Calculate the normal from the plane P spanned by start, the middle point at the higher one and goal
        if (middlePoints.Length == 2)
        {
            normal = Vector3.Cross(middlePoints[higherOne] - start, direction);
        }
        else
            normal = Vector3.Cross(middlePoints[0] - start, direction);

        normal.Normalize();
        Vector3 up = Vector3.Cross(direction, normal);
        up.Normalize();
        //(direction, normal, up) is now an orthonormal basis for P

        //When there are two middle points, shift the middle point that wasn't used to create P down on P
        if (middlePoints.Length == 2)
        {
            float coordinateForm = start.x * normal.x + start.y * normal.y + start.z * normal.z - middlePoints[1 - higherOne].x * normal.x - middlePoints[1 - higherOne].y * normal.y - middlePoints[1 - higherOne].z * normal.z;
            if (!FloatEq(coordinateForm, 0))
            {
                float l = coordinateForm / normal.y;
                middlePoints[1 - higherOne] = middlePoints[1 - higherOne] + l * Vector3.up;
            }
        }


        //Set the base transform matrixes
        Matrix4x4 planeToStandart = new Matrix4x4(direction,up,normal, new Vector4(0,0,0,1));
        Matrix4x4 standartToPlane = planeToStandart.inverse;

        //Transform the points into the new basis
        start = standartToPlane * new Vector4(start.x,start.y,start.z,1);
        goal = standartToPlane * new Vector4(goal.x, goal.y, goal.z, 1);
        for(int i = 0; i < middlePoints.Length; i++)
            middlePoints[i] = standartToPlane * new Vector4(middlePoints[i].x, middlePoints[i].y, middlePoints[i].z, 1);

        //Shift the middlepoints away from the obstacle
        if (middlePoints.Length == 2)
        {
            middlePoints[0] += new Vector3(-distanceToObstacle, distanceToObstacle, 0);
            middlePoints[1] += new Vector3(distanceToObstacle, distanceToObstacle, 0);
        }
        else
            middlePoints[0] += new Vector3(0,distanceToObstacle,0);
        /*
        g0.transform.position = planeToStandart * start;
        g1.transform.position = planeToStandart * middlePoints[0];
        g2.transform.position = planeToStandart * middlePoints[1];
        g3.transform.position = planeToStandart * goal;
        */
        Vector3[] curve = null;
        if (middlePoints.Length == 2)
        {
            //Curve 1
            controllPointsCurve1[0] = planeToStandart * start;
            controllPointsCurve1[2] = planeToStandart * middlePoints[0];

            Vector2 p1;
            if(start.x < middlePoints[0].x)
                p1 = new Vector2(start.x, middlePoints[0].y);
            else
                p1 = new Vector2(middlePoints[0].x, start.y);

            //g3.transform.position = planeToStandart * new Vector3(p1.x, p1.y, start.z);
            p1 = CalculateCentroid(start, p1, middlePoints[0]);
            controllPointsCurve1[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);

            //g0.transform.position = controllPointsCurve1[0];
            //g1.transform.position = controllPointsCurve1[1];
            //g2.transform.position = controllPointsCurve1[2];

            //Curve 3
            controllPointsCurve3[0] = planeToStandart * middlePoints[1];
            controllPointsCurve3[2] = planeToStandart * goal;

            Vector2 p2;
            if (goal.x > middlePoints[1].x)
                p2 = new Vector2(goal.x, middlePoints[1].y);
            else
                p2 = new Vector2(middlePoints[1].x, goal.y);
            p2 = CalculateCentroid(goal, p2, middlePoints[1]);
            controllPointsCurve3[1] = planeToStandart * new Vector3(p2.x, p2.y, start.z);

            //Curve 2 that connects Curve 1 and 3
            controllPointsCurve2[0] = controllPointsCurve1[2];
            controllPointsCurve2[3] = controllPointsCurve3[0];

            Vector2 aboveMiddle = CalculateLineIntersection(p1, (Vector2)middlePoints[0] - p1, p2, (Vector2)middlePoints[1] - p2);
            Vector2 aboveMiddleP1 = (Vector2)middlePoints[0] + (aboveMiddle - (Vector2)middlePoints[0]) / 2;
            controllPointsCurve2[1] = planeToStandart * new Vector3(aboveMiddleP1.x, aboveMiddleP1.y, start.z);

            Vector2 aboveMiddleP2 = (Vector2)middlePoints[1] + (aboveMiddle - (Vector2)middlePoints[1]) / 2;
            controllPointsCurve2[2] = planeToStandart * new Vector3(aboveMiddleP2.x, aboveMiddleP2.y, start.z);

            Vector3[] curve1 = Curve.CalculateBezierCurve(controllPointsCurve1, segmentCount / 3);
            Vector3[] curve2 = Curve.CalculateBezierCurve(controllPointsCurve2, segmentCount / 3);
            Vector3[] curve3 = Curve.CalculateBezierCurve(controllPointsCurve3, segmentCount / 3);
            curve = new Vector3[curve1.Length + curve2.Length + curve3.Length];
            curve1.CopyTo(curve, 0);
            curve2.CopyTo(curve, curve1.Length);
            curve3.CopyTo(curve, curve1.Length + curve2.Length);
        }
        else
        {
            //Curve 1
            controllPointsCurve1[0] = planeToStandart * start;
            controllPointsCurve1[2] = planeToStandart * middlePoints[0];
            Vector2 p1 = new Vector2(start.x, middlePoints[0].y);
            controllPointsCurve1[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);

            //Curve 3
            controllPointsCurve3[0] = planeToStandart * middlePoints[0];
            controllPointsCurve3[2] = planeToStandart * goal;
            p1 = new Vector2(goal.x, middlePoints[0].y);
            controllPointsCurve3[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);

            //Because we don't need a middle curve here, curve 2 isn't used

            Vector3[] curve1 = Curve.CalculateBezierCurve(controllPointsCurve1, segmentCount / 2);
            Vector3[] curve3 = Curve.CalculateBezierCurve(controllPointsCurve3, segmentCount / 2);

            curve = new Vector3[curve1.Length + curve3.Length];
            curve1.CopyTo(curve, 0);
            curve3.CopyTo(curve, curve1.Length);
        }

        return curve;
    }
}
