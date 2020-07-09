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

    

    public static Vector3[] startContinous(Vector3 start, Vector3 goal, GameObject[] pointVis = null)
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
        Vector3[] checkCurve = Curve.CalculateBezierCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, 4, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, 4, 0), goal }, 15);
        if (!Curve.CurveCollsionCheck(checkCurve))
        {
            return Curve.CalculateBezierCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, 4, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, 4, 0), goal }, 50);
        }


        //Genenerating the standart curve didn't work due to collsions. Try to set the controllpoints so, that no collion occures

        //Generate bounding box for above:
        Vector3 minPoint = center;
        Vector3 maxPoint = center;

        Collider[] colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center,new Vector3(0.2f,Math.Abs(start.y-goal.y)+20,distance/2.1f),Quaternion.LookRotation(direction,Vector3.up));

        foreach (Collider collider in colliders)
        {
            minPoint = new Vector3(Mathf.Min(minPoint.x,collider.bounds.min.x), Mathf.Min(minPoint.y, collider.bounds.min.y), Mathf.Min(minPoint.z, collider.bounds.min.z));
            maxPoint = new Vector3(Mathf.Max(maxPoint.x, collider.bounds.max.x), Mathf.Max(maxPoint.y, collider.bounds.max.y), Mathf.Max(maxPoint.z, collider.bounds.max.z));
        }

        if (minPoint == maxPoint)
        {
            minPoint += new Vector3(-1, -1, -1);
            maxPoint += new Vector3(1, 1, 1);
        }

        minPoint = new Vector3(minPoint.x, maxPoint.y, minPoint.z);

        //Generate bounding box for the sides:
        Vector3 minPointSide = center;
        Vector3 maxPointSide = center;

        colliders = LineControllScriptFrameShare.GetCollidorsFromObstacles(center + new Vector3(0,2,0), new Vector3(3, 4, distance / 2.1f), Quaternion.LookRotation(direction, Vector3.up));
        foreach (Collider collider in colliders)
        {
            minPointSide = new Vector3(Mathf.Min(minPoint.x, collider.bounds.min.x), Mathf.Min(minPoint.y, collider.bounds.min.y), Mathf.Min(minPoint.z, collider.bounds.min.z));
            maxPointSide = new Vector3(Mathf.Max(maxPoint.x, collider.bounds.max.x), Mathf.Max(maxPoint.y, collider.bounds.max.y), Mathf.Max(maxPoint.z, collider.bounds.max.z));
        }

        Vector3 reCenter = minPointSide + (maxPointSide - minPointSide) * 0.5f;
        Vector3 reExtend = maxPointSide - minPointSide + new Vector3(3, 0, 3);
        reExtend.y = standartHeight + 2;
        reExtend /= 2;
        reCenter.y = reExtend.y;

        Collider[] colllidersReScan = LineControllScriptFrameShare.GetCollidorsFromObstacles(reCenter, reExtend, Quaternion.identity);
        foreach (Collider collider in colllidersReScan)
        {
            minPointSide = new Vector3(Mathf.Min(minPointSide.x, collider.bounds.min.x), Mathf.Min(minPointSide.y, collider.bounds.min.y), Mathf.Min(minPointSide.z, collider.bounds.min.z));
            maxPointSide = new Vector3(Mathf.Max(maxPointSide.x, collider.bounds.max.x), Mathf.Max(maxPointSide.y, collider.bounds.max.y), Mathf.Max(maxPointSide.z, collider.bounds.max.z));
        }

        for (int i = 0; i < 5 && colliders.Length != colllidersReScan.Length; i++)
        {
            colliders = colllidersReScan;
            reCenter = minPointSide + (maxPointSide - minPointSide) * 0.5f;
            reExtend = maxPointSide - minPointSide + new Vector3(3, 0, 3);
            reExtend.y = standartHeight + 2;
            reExtend /= 2;
            reCenter.y = reExtend.y;
            colllidersReScan = LineControllScriptFrameShare.GetCollidorsFromObstacles(reCenter, reExtend, Quaternion.identity);
            foreach (Collider collider in colllidersReScan)
            {
                minPointSide = new Vector3(Mathf.Min(minPointSide.x, collider.bounds.min.x), Mathf.Min(minPointSide.y, collider.bounds.min.y), Mathf.Min(minPointSide.z, collider.bounds.min.z));
                maxPointSide = new Vector3(Mathf.Max(maxPointSide.x, collider.bounds.max.x), Mathf.Max(maxPointSide.y, collider.bounds.max.y), Mathf.Max(maxPointSide.z, collider.bounds.max.z));
            }
            
        }


        

        Vector3[] intersectionPointsAbove = calculateIntersectionAbove(start,goal,minPoint,maxPoint,direction);
        Vector3[] intersectionPointsSide = calculateIntersectionSide(start,goal, minPointSide, maxPointSide, direction,standartHeight);

        //check if the points are between start and goal and not to far above or to the side



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
            return CalculateJoinedCurveSide(start, intersectionPointsAbove, goal, 15);
        }

        else
        {
            Vector3 boundingBoxCenter = minPointSide + (maxPointSide - minPointSide) * 0.5f;
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
                Vector3 up = intersectionPointsSide[0] - boundingBoxCenterShifted + intersectionPointsSide[1] - boundingBoxCenterShifted;
                up.y = standartHeight;
                up.Normalize();

                //return CalculateJoinedCurve(start, intersectionPointsSide, goal, up);
                //return CalculateJoinedCurveSide(start, intersectionPointsSide, goal, pointVis[0], pointVis[1], pointVis[2], pointVis[3]);
                return CalculateJoinedCurveSide(start, intersectionPointsSide, goal, 15);
            }

        }

        return Curve.CalculateBezierCurve(controllPoints, 50);

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

        float higherY = Mathf.Max(start.y , goal.y);

        //Determine the collsion points for left/right
        Vector3 leftLower = new Vector3(minPoint.x, higherY + standartHeight, minPoint.z);
        Vector3 leftUpper = new Vector3(minPoint.x, higherY + standartHeight, maxPoint.z);
        Vector3 rightUpper = new Vector3(maxPoint.x, higherY + standartHeight, maxPoint.z);
        Vector3 rightLower = new Vector3(maxPoint.x, higherY + standartHeight, minPoint.z);

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

    static Vector3[] CalculateJoinedCurve(Vector3 start, Vector3[] middlePoints, Vector3 goal, Vector3 up)
    {
        Vector3[] controllPointsCurve1 = new Vector3[3];
        Vector3[] controllPointsCurve2 = new Vector3[4];
        Vector3[] controllPointsCurve3 = new Vector3[3];

        Vector3 direction = middlePoints[1] - middlePoints[0];
        direction.Normalize();
        middlePoints[0] -= direction * distanceToObstacle;
        middlePoints[0] += up * distanceToObstacle;
        middlePoints[1] += direction * distanceToObstacle;
        middlePoints[1] += up * distanceToObstacle;

        //Curve 1
        controllPointsCurve1[0] = start;
        controllPointsCurve1[2] = middlePoints[0];
        float hypothenuse = Vector3.Distance(controllPointsCurve1[0], controllPointsCurve1[2]);
        float alpha = (float)(Math.PI * Vector3.Angle(up, controllPointsCurve1[2] - controllPointsCurve1[0]) /180);
        Vector3 p1 = controllPointsCurve1[0] + up * (float)(hypothenuse * Math.Cos(alpha));
        controllPointsCurve1[1] = CalculateCentroid(controllPointsCurve1[0], p1 , controllPointsCurve1[2]);



        //Curve 3
        controllPointsCurve3[0] = middlePoints[1];
        controllPointsCurve3[2] = goal;
        hypothenuse = Vector3.Distance(controllPointsCurve3[0], controllPointsCurve3[2]);
        alpha = (float)(Math.PI * Vector3.Angle(up, controllPointsCurve3[0] - controllPointsCurve3[2]) / 180);
        controllPointsCurve3[1] = CalculateCentroid(controllPointsCurve3[2], controllPointsCurve3[2] + up * hypothenuse * (float)Math.Cos(alpha), controllPointsCurve3[0]);

        //Curve 2 that connects Curve 1 and 3
        controllPointsCurve2[0] = controllPointsCurve1[2];
        controllPointsCurve2[3] = controllPointsCurve3[0];

        Vector3 aboveMiddle = CalculateLineIntersection(controllPointsCurve1[1], controllPointsCurve1[2] - controllPointsCurve1[1], controllPointsCurve3[1], controllPointsCurve3[0] - controllPointsCurve3[1]);
        controllPointsCurve2[1] = controllPointsCurve2[0] + (aboveMiddle - controllPointsCurve2[0]) / 2;
        controllPointsCurve2[2] = controllPointsCurve2[3] + (aboveMiddle - controllPointsCurve2[3]) / 2;

        Vector3[] curve1 = Curve.CalculateBezierCurve(controllPointsCurve1, 20);
        Vector3[] curve2 = Curve.CalculateBezierCurve(controllPointsCurve2, 20);
        Vector3[] curve3 = Curve.CalculateBezierCurve(controllPointsCurve3, 20);
        Vector3[] curve = new Vector3[curve1.Length + curve2.Length + curve3.Length];
        curve1.CopyTo(curve, 0);
        curve2.CopyTo(curve, curve1.Length);
        curve3.CopyTo(curve, curve1.Length + curve2.Length);

        return curve;
    }

    public static Vector3[] CalculateJoinedCurveSide(Vector3 start, Vector3[] middlePoints, Vector3 goal, int segmentCount, GameObject g0 = null, GameObject g1 = null, GameObject g2 = null, GameObject g3 = null)
    {
        Vector3[] controllPointsCurve1 = new Vector3[3];
        Vector3[] controllPointsCurve2 = new Vector3[4];
        Vector3[] controllPointsCurve3 = new Vector3[3];

        Vector3 direction = goal - start;
        direction.Normalize();
        Vector3 normal;
        if (start.y > goal.y)
            normal = Vector3.Cross(middlePoints[0] - start, direction);
        else
            normal = Vector3.Cross(middlePoints[1] - start, direction);

        normal.Normalize();
        Vector3 up = Vector3.Cross(direction, normal);
        up.Normalize();
        //(direction, normal, up) is an orthonormal basis for plane P spanned by start middlepoint[0] and goal

            if (start.y > goal.y)
            {
                //calculate new middlePoints[1] in such a way that middlePoints[1] is on P
                float coordinateForm = start.x * normal.x + start.y * normal.y + start.z * normal.z - middlePoints[1].x * normal.x - middlePoints[1].y * normal.y - middlePoints[1].z * normal.z;
                if (!FloatEq(coordinateForm, 0))
                {
                    float l = coordinateForm / normal.y;
                    middlePoints[1] = middlePoints[1] + l * Vector3.up;
                }
            }
            else
            {
                float coordinateForm = start.x * normal.x + start.y * normal.y + start.z * normal.z - middlePoints[0].x * normal.x - middlePoints[0].y * normal.y - middlePoints[0].z * normal.z;
                if (!FloatEq(coordinateForm, 0))
                {
                    float l = coordinateForm / normal.y;
                    middlePoints[0] = middlePoints[0] + l * Vector3.up;
                }
            }


        Matrix4x4 planeToStandart = new Matrix4x4(direction,up,normal, new Vector4(0,0,0,1));
        Matrix4x4 standartToPlane = planeToStandart.inverse;

        //Transform the points into the new basis
        start = standartToPlane * new Vector4(start.x,start.y,start.z,1);
        goal = standartToPlane * new Vector4(goal.x, goal.y, goal.z, 1);
        middlePoints[0] = standartToPlane * new Vector4(middlePoints[0].x, middlePoints[0].y, middlePoints[0].z, 1);
        middlePoints[1] = standartToPlane * new Vector4(middlePoints[1].x, middlePoints[1].y, middlePoints[1].z, 1);

        //Shift the middlepoints away from the obstacle
        middlePoints[0] += new Vector3(-distanceToObstacle,distanceToObstacle,0);
        middlePoints[1] += new Vector3(distanceToObstacle, distanceToObstacle, 0);

        /*
        g0.transform.position = planeToStandart * start;
        g1.transform.position = planeToStandart * middlePoints[0];
        g2.transform.position = planeToStandart * middlePoints[1];
        g3.transform.position = planeToStandart * goal;
        */

        //Curve 1
        controllPointsCurve1[0] = planeToStandart * start;
        controllPointsCurve1[2] = planeToStandart * middlePoints[0];

        Vector2 p1 = new Vector2(start.x , middlePoints[0].y);
        //g3.transform.position = planeToStandart * new Vector3(p1.x, p1.y, start.z);
        p1 = CalculateCentroid(start, p1, middlePoints[0]);
        controllPointsCurve1[1] = planeToStandart * new Vector3(p1.x,p1.y,start.z);

        //g0.transform.position = controllPointsCurve1[0];
        //g1.transform.position = controllPointsCurve1[1];
        //g2.transform.position = controllPointsCurve1[2];

        //Curve 3
        controllPointsCurve3[0] = planeToStandart * middlePoints[1];
        controllPointsCurve3[2] = planeToStandart * goal;

        Vector2 p2 = new Vector2(goal.x, middlePoints[1].y);
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

        Vector3[] curve1 = Curve.CalculateBezierCurve(controllPointsCurve1, segmentCount/3);
        Vector3[] curve2 = Curve.CalculateBezierCurve(controllPointsCurve2, segmentCount/3);
        Vector3[] curve3 = Curve.CalculateBezierCurve(controllPointsCurve3, segmentCount/3);
        Vector3[] curve = new Vector3[curve1.Length + curve2.Length + curve3.Length];
        curve1.CopyTo(curve, 0);
        curve2.CopyTo(curve, curve1.Length);
        curve3.CopyTo(curve, curve1.Length + curve2.Length);

        return curve;
    }
}
