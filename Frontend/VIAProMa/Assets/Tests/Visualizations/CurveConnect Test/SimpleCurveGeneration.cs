using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Jobs;
using Unity.Collections;

public struct BoundingBoxes
{
    public Vector3 minPointAbove;
    public Vector3 maxPointAbove;
    public Vector3 minPointSide;
    public Vector3 maxPointSide;
    public int curveIndex;
}


public class SimpleCurveGerneration : CurveGenerator
{
    public static float distanceToObstacle = 0.5f;
    

    public static Vector3[] StandartCurve(Vector3 start, Vector3 goal, int segmentCount, float height)
    {
        float higherOne = start.y > goal.y ? start.y : goal.y;

        Vector3 startAdjusted = new Vector3(start.x, higherOne, start.z);
        Vector3 goalAdjusted = new Vector3(goal.x, higherOne, goal.z);
        Vector3 direction = goalAdjusted - startAdjusted;
        direction.Normalize();
        float distance = Vector3.Distance(startAdjusted, goalAdjusted);
        Vector3 center = startAdjusted + direction * distance / 2;

        return CalculateBezierCurve(new Vector3[] { start, startAdjusted + direction * distance / 2.5f + new Vector3(0, height, 0), goalAdjusted - direction * distance / 2.5f + new Vector3(0, height, 0), goal }, 50);
    }

    public static Vector3[] TryToUseStandartCurve(GameObject startObject, GameObject goalObject, int curveSegmentCount)
    {
        //Priority 1: Try to draw the standart curve
        Vector3 start = startObject.transform.position;
        Vector3 goal = goalObject.transform.position;
        Vector3[] standartCurve = StandartCurve(start, goal, curveSegmentCount, 0.5f);
        if (!CurveCollsionCheck(standartCurve, startObject, goalObject))
        {
            return standartCurve;
        }
        return null;
    }

    public static BoundingBoxes CalculateBoundingBoxes(GameObject startObject, GameObject goalObject)
    {
        Vector3 start = startObject.transform.position;
        Vector3 goal = goalObject.transform.position;

        //float higherOne = start.y > goal.y ? start.y : goal.y;



        Vector3 direction = goal - start;
        direction.Normalize();

        float distance = Vector3.Distance(start, goal);

        Vector3 center = start + direction * distance / 2;

        //TODO put in parameter
        float standartHeight = 0.5f;

        //Genenerating the standart curve didn't work due to collsions. Try to set the controllpoints so, that no collisions occure

        //Sets min and max in such a way that they include as many of the obstacles as possible but never start or goal
        void SetMinMax(Collider[] colliders, ref Vector3 min, ref Vector3 max)
        {
            foreach (Collider collider in colliders)
            {
                min = Vector3.Min(min, collider.bounds.min);
                max = Vector3.Max(max, collider.bounds.max);
                float correctionOffset = distance * 0.1f;

                if (start.x >= min.x && start.z >= min.z && start.x <= max.x && start.z <= max.z)
                {
                    Vector3 startGoal = goal - start;
                    Vector2 correction = new Vector2(startGoal.x, startGoal.z);
                    correction.Normalize();
                    if (Mathf.Abs(correction.x) > Mathf.Abs(correction.y))
                        correction = new Vector2(Mathf.Sign(correction.x), 0);
                    else
                        correction = new Vector2(0, Mathf.Sign(correction.y));
                    if (correction.x < 0)
                        max.x = start.x - correctionOffset;
                    else if (correction.y < 0)
                        max.z = start.z - correctionOffset;
                    else if (correction.x > 0)
                        min.x = start.x + correctionOffset;
                    else
                        min.z = start.z + correctionOffset;
                }
                if (goal.x >= min.x && goal.z >= min.z && goal.x <= max.x && goal.z <= max.z)
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
        void ScanForObstaclesAbove(ref Vector3 min, ref Vector3 max)
        {
            float lowerY = start.y < goal.y ? start.y : goal.y;
            Vector3 directionAdjusted = new Vector3(direction.x, 0, direction.z);
            directionAdjusted.Normalize();
            float distanceAdjusted = (new Vector3(start.x, 0, start.z) - new Vector3(goal.x, 0, goal.z)).magnitude;
            Vector3 centerAdjusted = new Vector3(start.x, lowerY, start.z) + directionAdjusted * distanceAdjusted / 2;
            Vector3 startBoxHalfExtend = new Vector3(1.1f * distanceToObstacle, (Math.Abs(start.y - goal.y) + standartHeight + distanceToObstacle * 3) / 2, distance / 2.1f);

            Collider[] ReScancolliders = GetCollidorsFromObstacles(centerAdjusted + new Vector3(0, startBoxHalfExtend.y, 0),
                startBoxHalfExtend, Quaternion.LookRotation(directionAdjusted, Vector3.up), startObject, goalObject);
            SetMinMax(ReScancolliders, ref min, ref max);
            int oldLength = ReScancolliders.Length;
            int i = 1;
            bool newObstacles = false;
            do
            {
                newObstacles = false;
                startBoxHalfExtend.y = (max.y + 3 * distanceToObstacle) / 2;
                ReScancolliders = GetCollidorsFromObstacles(centerAdjusted + new Vector3(0, startBoxHalfExtend.y, 0),
                    startBoxHalfExtend, Quaternion.LookRotation(directionAdjusted, Vector3.up), startObject, goalObject);
                ReScancolliders = GetCollidorsFromObstacles(centerAdjusted + new Vector3(0, startBoxHalfExtend.y, 0), startBoxHalfExtend, Quaternion.LookRotation(directionAdjusted, Vector3.up), startObject, goalObject);
                if (ReScancolliders.Length > oldLength)
                {
                    SetMinMax(ReScancolliders, ref min, ref max);
                    newObstacles = true;
                }
                oldLength = ReScancolliders.Length;
                i++;
            } while (i <= 100 && newObstacles);

        }
        void ScanForObstaclesSide(ref Vector3 min, ref Vector3 max)
        {
            Vector3 normal = Vector3.Cross(direction, Vector3.up);
            normal.Normalize();
            Vector3 up = Vector3.Cross(direction, normal);
            if (up.y < 0)
                up *= -1;
            Vector3 startBoxHalfExtend = new Vector3(distanceToObstacle, (standartHeight + distanceToObstacle) / 2, distance / 2.1f);

            //Scan with the starting box
            Collider[] ReScancolliders = GetCollidorsFromObstacles(center + up * startBoxHalfExtend.y, startBoxHalfExtend, Quaternion.LookRotation(direction, up), startObject, goalObject);
            SetMinMax(ReScancolliders, ref min, ref max);

            //Rescan as long as new collider show up or the maximal number of scans is reached
            int oldLength = ReScancolliders.Length;
            int i = 1;
            bool newObstacles = false;
            do
            {
                newObstacles = false;
                Vector3 boxCenter = min + (max - min) * 0.5f;
                Vector3 halfExtend = max - min + new Vector3(10 * distanceToObstacle, 0, 10 * distanceToObstacle);
                halfExtend.y = standartHeight + distanceToObstacle;
                halfExtend /= 2;
                boxCenter.y = halfExtend.y;
                ReScancolliders = GetCollidorsFromObstacles(boxCenter, halfExtend, Quaternion.identity, startObject, goalObject);
                if (ReScancolliders.Length > oldLength)
                {
                    SetMinMax(ReScancolliders, ref min, ref max);
                    newObstacles = true;
                }
                oldLength = ReScancolliders.Length;
                i++;
            }
            while (i <= 100 && newObstacles);
        }
        //Generate bounding box for above:
        Vector3 minPointAbove = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPointAbove = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        ScanForObstaclesAbove(ref minPointAbove, ref maxPointAbove);
        Vector3 extendVector = new Vector3(distanceToObstacle, distanceToObstacle, distanceToObstacle);
        minPointAbove -= extendVector;
        maxPointAbove += extendVector;

        //Generate bounding box for the sides:
        Vector3 minPointSide = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPointSide = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        ScanForObstaclesSide(ref minPointSide, ref maxPointSide);
        minPointSide -= extendVector;
        maxPointSide += extendVector;
        BoundingBoxes result = new BoundingBoxes();
        result.minPointAbove = minPointAbove;
        result.maxPointAbove = maxPointAbove;
        result.minPointSide = minPointSide;
        result.maxPointSide = maxPointSide;
        return result;
    }

    public static Vector3[] StartGeneration(Vector3 start, Vector3 goal, BoundingBoxes boundingBoxes , int curveSegmentCount)
    {
        float standartHeight = 0.5f;

        Vector3[] intersectionPointsAbove = calculateIntersectionAbove(start,goal, boundingBoxes.minPointAbove, boundingBoxes.maxPointAbove);
        Vector3[] intersectionPointsSide = calculateIntersectionSide(start,goal, boundingBoxes.minPointSide, boundingBoxes.maxPointSide,standartHeight);



        float distanceAbove = CurveLength(new Vector3[] { start, intersectionPointsAbove[0], intersectionPointsAbove[1], goal});
        float distanceSide;
        if (intersectionPointsSide.Length == 1)
            distanceSide = CurveLength(new Vector3[] { start, intersectionPointsSide[0], goal });
        else if(intersectionPointsSide.Length == 2)
            distanceSide = CurveLength(new Vector3[] { start, intersectionPointsSide[0], intersectionPointsSide[1], goal });
        else
            return StandartCurve(start,goal,curveSegmentCount,standartHeight);

        Vector3[] controllPoints = new Vector3[0];
        if (distanceAbove < 1.5 * distanceSide)
        {
            return CalculateJoinedCurve(start, intersectionPointsAbove, goal, curveSegmentCount);
        }
        else
        {
            return CalculateJoinedCurve(start, intersectionPointsSide, goal, curveSegmentCount);
        }
        
    }

    public static Vector3[] calculateIntersectionAbove(Vector3 start, Vector3 goal, Vector3 minPoint, Vector3 maxPoint)
    {
        Vector3 startOnMaxHeight = new Vector3(start.x, maxPoint.y, start.z);
        Vector3 direction = goal - start;
        Vector3 directionAdjusted = new Vector3(direction.x, 0, direction.z);
        directionAdjusted.Normalize();
        //Calculate the controll points for the way above
        //calculate the potential intersections from the line spanned by startOnMaxHeight+l*direction with the 4 lines that form the rectangle spanned by min and max
        Vector3[] potentialPoints = new Vector3[4];

        potentialPoints[0] = startOnMaxHeight + (minPoint.z - start.z) / directionAdjusted.z * directionAdjusted;
        potentialPoints[1] = startOnMaxHeight + (minPoint.x - start.x) / directionAdjusted.x * directionAdjusted;

        potentialPoints[2] = startOnMaxHeight + (maxPoint.z - start.z) / directionAdjusted.z * directionAdjusted;
        potentialPoints[3] = startOnMaxHeight + (maxPoint.x - start.x) / directionAdjusted.x * directionAdjusted;



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
    public static Vector3[] calculateIntersectionSide(Vector3 start, Vector3 goal, Vector3 minPoint, Vector3 maxPoint, float standartHeight)
    {        
        Vector2 CalculatePlacmentToBoundingbox(Vector3 point, Vector3 min, Vector3 max)
        {
            Vector3 extendVector = new Vector3(distanceToObstacle, distanceToObstacle, distanceToObstacle);
            min += extendVector;
            max -= extendVector;

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
            float approximateWayLeft;
            float approximateWayRight;
            if (edgesSorted[false].Count == 2)
            {
                approximateWayLeft = Vector3.Distance(start, edgesSorted[false][0]) + Vector3.Distance(goal, edgesSorted[false][1]);
                approximateWayRight = Vector3.Distance(start, edgesSorted[true][0]) + Vector3.Distance(goal, edgesSorted[true][1]);
            }
            else
            {
                approximateWayLeft = Vector3.Distance(start, edgesSorted[false][0]) + Vector3.Distance(edgesSorted[false][0], goal);
                approximateWayRight = Vector3.Distance(start, edgesSorted[true][0]) + Vector3.Distance(edgesSorted[true][0], goal);
            }
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

        Vector3[] curve = null;

        

        if (middlePoints.Length == 2)
        {
            curve = CurveWithTwoPoints(start,goal, planeToStandart, middlePoints, segmentCount);
        }
        else
        {
            curve = CurveWithOnePoint(start, goal, planeToStandart, middlePoints[0], segmentCount);
        }

        return curve;
    }

    static Vector3[] CurveWithTwoPoints(Vector3 start, Vector3 goal, Matrix4x4 planeToStandart, Vector3[] middlePoints, int segmentCount)
    {
        Vector3[] controllPointsCurve1 = new Vector3[3];
        Vector3[] controllPointsCurve2 = new Vector3[4];
        Vector3[] controllPointsCurve3 = new Vector3[3];
        //Curve 1
        controllPointsCurve1[0] = planeToStandart * start;
        controllPointsCurve1[2] = planeToStandart * middlePoints[0];

        Vector2 p1;
        if (start.x < middlePoints[0].x)
            p1 = new Vector2(start.x, middlePoints[0].y);
        else
            p1 = new Vector2(middlePoints[0].x, start.y);

        p1 = CalculateCentroid(start, p1, middlePoints[0]);
        controllPointsCurve1[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);

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
        Vector2 aboveMiddleP1;
        Vector2 aboveMiddleP2;
        Vector3[] curve;
        if (aboveMiddle.y < middlePoints[0].y || aboveMiddle.y < middlePoints[1].y)
        {
            return CurveWithOnePoint(start, goal, planeToStandart, middlePoints[0].y > middlePoints[1].y ? middlePoints[0] : middlePoints[1], segmentCount);
        }
        aboveMiddleP1 = (Vector2)middlePoints[0] + (aboveMiddle - (Vector2)middlePoints[0]) / 2;
        controllPointsCurve2[1] = planeToStandart * new Vector3(aboveMiddleP1.x, aboveMiddleP1.y, start.z);

        aboveMiddleP2 = (Vector2)middlePoints[1] + (aboveMiddle - (Vector2)middlePoints[1]) / 2;
        controllPointsCurve2[2] = planeToStandart * new Vector3(aboveMiddleP2.x, aboveMiddleP2.y, start.z);

        Vector3[] curve1 = CurveGenerator.CalculateBezierCurve(controllPointsCurve1, segmentCount / 3);
        Vector3[] curve2 = CurveGenerator.CalculateBezierCurve(controllPointsCurve2, segmentCount / 3);
        Vector3[] curve3 = CurveGenerator.CalculateBezierCurve(controllPointsCurve3, segmentCount / 3);
        curve = new Vector3[curve1.Length + curve2.Length + curve3.Length];
        curve1.CopyTo(curve, 0);
        curve2.CopyTo(curve, curve1.Length);
        curve3.CopyTo(curve, curve1.Length + curve2.Length);
        return curve;
    }

    static Vector3[] CurveWithOnePoint(Vector3 start, Vector3 goal, Matrix4x4 planeToStandart, Vector3 middlePoint, int segmentCount)
    {
        Vector3[] controllPointsCurve1 = new Vector3[3];
        Vector3[] ControllPointsCurve2 = new Vector3[3];
        Vector3[] curve;

        //Curve 1
        controllPointsCurve1[0] = planeToStandart * start;
        controllPointsCurve1[2] = planeToStandart * middlePoint;
        Vector2 p1 = new Vector2(start.x, middlePoint.y);
        controllPointsCurve1[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);

        //Curve 2
        ControllPointsCurve2[0] = planeToStandart * middlePoint;
        ControllPointsCurve2[2] = planeToStandart * goal;
        p1 = new Vector2(goal.x, middlePoint.y);
        ControllPointsCurve2[1] = planeToStandart * new Vector3(p1.x, p1.y, start.z);


        Vector3[] curve1 = CurveGenerator.CalculateBezierCurve(controllPointsCurve1, segmentCount / 2);
        Vector3[] curve3 = CurveGenerator.CalculateBezierCurve(ControllPointsCurve2, segmentCount / 2);

        curve = new Vector3[curve1.Length + curve3.Length];
        curve1.CopyTo(curve, 0);
        curve3.CopyTo(curve, curve1.Length);
        return curve;
    }
}

struct SimpleCurveGenerationJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<BoundingBoxes> boxes;
    [ReadOnly]
    public NativeArray<Vector3> start;
    [ReadOnly]
    public NativeArray<Vector3> goal;

    //I am truly sorry for this, but this Unity version doesn't allow any form of sane managment of two dimensional native data. If the unity Version ever gets Updated, this can all be replaced by a single native hashmap
    
    public NativeArray<Vector3> curvePoint0;
    public NativeArray<Vector3> curvePoint1;
    public NativeArray<Vector3> curvePoint2;
    public NativeArray<Vector3> curvePoint3;
    public NativeArray<Vector3> curvePoint4;
    public NativeArray<Vector3> curvePoint5;
    public NativeArray<Vector3> curvePoint6;
    public NativeArray<Vector3> curvePoint7;
    public NativeArray<Vector3> curvePoint8;
    public NativeArray<Vector3> curvePoint9;
    public NativeArray<Vector3> curvePoint10;
    public NativeArray<Vector3> curvePoint11;
    public NativeArray<Vector3> curvePoint12;
    public NativeArray<Vector3> curvePoint13;
    public NativeArray<Vector3> curvePoint14;
    public NativeArray<Vector3> curvePoint15;
    public NativeArray<Vector3> curvePoint16;
    public NativeArray<Vector3> curvePoint17;
    public NativeArray<Vector3> curvePoint18;
    public NativeArray<Vector3> curvePoint19;
    public NativeArray<Vector3> curvePoint20;
    public NativeArray<Vector3> curvePoint21;
    public NativeArray<Vector3> curvePoint22;
    public NativeArray<Vector3> curvePoint23;
    public NativeArray<Vector3> curvePoint24;
    public NativeArray<Vector3> curvePoint25;
    public NativeArray<Vector3> curvePoint26;
    public NativeArray<Vector3> curvePoint27;
    public NativeArray<Vector3> curvePoint28;
    public NativeArray<Vector3> curvePoint29;
    public NativeArray<Vector3> curvePoint30;
    public NativeArray<Vector3> curvePoint31;
    public NativeArray<Vector3> curvePoint32;
    public NativeArray<Vector3> curvePoint33;
    public NativeArray<Vector3> curvePoint34;
    public NativeArray<Vector3> curvePoint35;
    public NativeArray<Vector3> curvePoint36;
    public NativeArray<Vector3> curvePoint37;
    public NativeArray<Vector3> curvePoint38;
    public NativeArray<Vector3> curvePoint39;
    public NativeArray<Vector3> curvePoint40;
    public NativeArray<Vector3> curvePoint41;
    public NativeArray<Vector3> curvePoint42;
    public NativeArray<Vector3> curvePoint43;
    public NativeArray<Vector3> curvePoint44;
    public NativeArray<Vector3> curvePoint45;
    public NativeArray<Vector3> curvePoint46;
    public NativeArray<Vector3> curvePoint47;
    public NativeArray<Vector3> curvePoint48;
    public NativeArray<Vector3> curvePoint49;
    public NativeArray<Vector3> curvePoint50;
    public NativeArray<Vector3> curvePoint51;
    public NativeArray<Vector3> curvePoint52;
    public NativeArray<Vector3> curvePoint53;
    public NativeArray<Vector3> curvePoint54;
    public NativeArray<Vector3> curvePoint55;
    public NativeArray<Vector3> curvePoint56;
    public NativeArray<Vector3> curvePoint57;
    public NativeArray<Vector3> curvePoint58;
    public NativeArray<Vector3> curvePoint59;
    //public NativeArray<Vector3> curvePoint60;
    //public NativeArray<Vector3> curvePoint61;
    //public NativeArray<Vector3> curvePoint62;

    public void Execute(int index)
    {
        float standartHeight = 0.5f;
        Vector3[] curve;

        Vector3[] intersectionPointsAbove = SimpleCurveGerneration.calculateIntersectionAbove(start[index], goal[index], boxes[index].minPointAbove, boxes[index].maxPointAbove);
        Vector3[] intersectionPointsSide = SimpleCurveGerneration.calculateIntersectionSide(start[index], goal[index], boxes[index].minPointSide, boxes[index].maxPointSide, standartHeight);



        float distanceAbove = CurveGenerator.CurveLength(new Vector3[] { start[index], intersectionPointsAbove[0], intersectionPointsAbove[1], goal[index] });
        float distanceSide;
        if (intersectionPointsSide.Length == 1)
            distanceSide = CurveGenerator.CurveLength(new Vector3[] { start[index], intersectionPointsSide[0], goal[index] });
        else if (intersectionPointsSide.Length == 2)
            distanceSide = CurveGenerator.CurveLength(new Vector3[] { start[index], intersectionPointsSide[0], intersectionPointsSide[1], goal[index] });
        else
        {
            curve = SimpleCurveGerneration.StandartCurve(start[index], goal[index], 60, standartHeight);
            //result[index] = new NativeArray<Vector3>(curve.Length,Allocator.TempJob);
            //result[index].CopyFrom(curve);
            //result.SetValue(curve,index);
            //for (int i = 0; i < dim2; i++)
            //{
            //    result.oneDArray[offset + i] = curve[i];
            //}
            SetResult(curve, index);
            return;
        }
            

        Vector3[] controllPoints = new Vector3[0];
        
        if (distanceAbove < 1.5 * distanceSide)
        {
            curve = SimpleCurveGerneration.CalculateJoinedCurve(start[index], intersectionPointsAbove, goal[index], 60);
            //result[index] = new NativeArray<Vector3>(curve.Length, Allocator.TempJob);
            //result[index].CopyFrom(curve);
            //result.SetValue(curve,index);
            //for (int i = 0; i < dim2; i++)
            //{
            //    result.oneDArray[offset + i] = curve[i];
            //}
            SetResult(curve, index);
            return;
        }
        else
        {
            curve = SimpleCurveGerneration.CalculateJoinedCurve(start[index], intersectionPointsSide, goal[index], 60);
            //result[index] = new NativeArray<Vector3>(curve.Length, Allocator.TempJob);
            //result[index].CopyFrom(curve);
            //result.SetValue(curve, index);
            //for (int i = 0; i < dim2; i++)
            //{
            //    result.oneDArray[offset + i] = curve[i];
            //}
            SetResult(curve, index);
            return;
        }
    }

    private void SetResult(Vector3[] curve, int index)
    {
        curvePoint0[index] = curve[0];
        curvePoint1[index] = curve[1];
        curvePoint2[index] = curve[2];
        curvePoint3[index] = curve[3];
        curvePoint4[index] = curve[4];
        curvePoint5[index] = curve[5];
        curvePoint6[index] = curve[6];
        curvePoint7[index] = curve[7];
        curvePoint8[index] = curve[8];
        curvePoint9[index] = curve[9];
        curvePoint10[index] = curve[10];
        curvePoint11[index] = curve[11];
        curvePoint12[index] = curve[12];
        curvePoint13[index] = curve[13];
        curvePoint14[index] = curve[14];
        curvePoint15[index] = curve[15];
        curvePoint16[index] = curve[16];
        curvePoint17[index] = curve[17];
        curvePoint18[index] = curve[18];
        curvePoint19[index] = curve[19];
        curvePoint20[index] = curve[20];
        curvePoint21[index] = curve[21];
        curvePoint22[index] = curve[22];
        curvePoint23[index] = curve[23];
        curvePoint24[index] = curve[24];
        curvePoint25[index] = curve[25];
        curvePoint26[index] = curve[26];
        curvePoint27[index] = curve[27];
        curvePoint28[index] = curve[28];
        curvePoint29[index] = curve[29];
        curvePoint30[index] = curve[30];
        curvePoint31[index] = curve[31];
        curvePoint32[index] = curve[32];
        curvePoint33[index] = curve[33];
        curvePoint34[index] = curve[34];
        curvePoint35[index] = curve[35];
        curvePoint36[index] = curve[36];
        curvePoint37[index] = curve[37];
        curvePoint38[index] = curve[38];
        curvePoint39[index] = curve[39];
        curvePoint40[index] = curve[40];
        curvePoint41[index] = curve[41];
        curvePoint42[index] = curve[42];
        curvePoint43[index] = curve[43];
        curvePoint44[index] = curve[44];
        curvePoint45[index] = curve[45];
        curvePoint46[index] = curve[46];
        curvePoint47[index] = curve[47];
        curvePoint48[index] = curve[48];
        curvePoint49[index] = curve[49];
        curvePoint50[index] = curve[50];
        curvePoint51[index] = curve[51];
        curvePoint52[index] = curve[52];
        curvePoint53[index] = curve[53];
        curvePoint54[index] = curve[54];
        curvePoint55[index] = curve[55];
        curvePoint56[index] = curve[56];
        curvePoint57[index] = curve[57];
        curvePoint58[index] = curve[58];
        curvePoint59[index] = curve[59];
        //curvePoint60[index] = curve[60];
        //curvePoint61[index] = curve[61];
        //curvePoint62[index] = curve[62];

    }

    public Vector3[] ReadResult(int index)
    {
        Vector3[] curve = new Vector3[60];
        curve[0] = curvePoint0[index];
        curve[1] = curvePoint1[index];
        curve[2] = curvePoint2[index];
        curve[3] = curvePoint3[index];
        curve[4] = curvePoint4[index];
        curve[5] = curvePoint5[index];
        curve[6] = curvePoint6[index];
        curve[7] = curvePoint7[index];
        curve[8] = curvePoint8[index];
        curve[9] = curvePoint9[index];
        curve[10] = curvePoint10[index];
        curve[11] = curvePoint11[index];
        curve[12] = curvePoint12[index];
        curve[13] = curvePoint13[index];
        curve[14] = curvePoint14[index];
        curve[15] = curvePoint15[index];
        curve[16] = curvePoint16[index];
        curve[17] = curvePoint17[index];
        curve[18] = curvePoint18[index];
        curve[19] = curvePoint19[index];
        curve[20] = curvePoint20[index];
        curve[21] = curvePoint21[index];
        curve[22] = curvePoint22[index];
        curve[23] = curvePoint23[index];
        curve[24] = curvePoint24[index];
        curve[25] = curvePoint25[index];
        curve[26] = curvePoint26[index];
        curve[27] = curvePoint27[index];
        curve[28] = curvePoint28[index];
        curve[29] = curvePoint29[index];
        curve[30] = curvePoint30[index];
        curve[31] = curvePoint31[index];
        curve[32] = curvePoint32[index];
        curve[33] = curvePoint33[index];
        curve[34] = curvePoint34[index];
        curve[35] = curvePoint35[index];
        curve[36] = curvePoint36[index];
        curve[37] = curvePoint37[index];
        curve[38] = curvePoint38[index];
        curve[39] = curvePoint39[index];
        curve[40] = curvePoint40[index];
        curve[41] = curvePoint41[index];
        curve[42] = curvePoint42[index];
        curve[43] = curvePoint43[index];
        curve[44] = curvePoint44[index];
        curve[45] = curvePoint45[index];
        curve[46] = curvePoint46[index];
        curve[47] = curvePoint47[index];
        curve[48] = curvePoint48[index];
        curve[49] = curvePoint49[index];
        curve[50] = curvePoint50[index];
        curve[51] = curvePoint51[index];
        curve[52] = curvePoint52[index];
        curve[53] = curvePoint53[index];
        curve[54] = curvePoint54[index];
        curve[55] = curvePoint55[index];
        curve[56] = curvePoint56[index];
        curve[57] = curvePoint57[index];
        curve[58] = curvePoint58[index];
        curve[59] = curvePoint59[index];
        //curve[60] = curvePoint60[index];
        //curve[61] = curvePoint61[index];
        //curve[62] = curvePoint62[index];
        return curve;
    }
}

public struct TwoDNativeArray<T> where T:struct
{
    public TwoDNativeArray(int dim1, int dim2, Allocator allocator)
    {
        oneDArray = new NativeArray<T>(dim1*dim2, allocator);
        this.dim1 = dim1;
        this.dim2 = dim2;
    }

    public void Write(int x, int y, T data)
    {
        if (x < dim1)
        {
            if (y < dim2)
            {
                oneDArray[x * dim2 + y] = data;
            }
            else
            {
                throw new Exception("TwoDNativeArray dim2 out of bounds");
            }
        }
        else
        {
            throw new Exception("TwoDNativeArray dim1 out of bounds");
        }
    }

    public T Read(int x, int y)
    {
        if (x < dim1)
        {
            if (y < dim2)
            {
                return oneDArray[x * dim2 + y];
            }
            else
            {
                throw new Exception("TwoDNativeArray dim2 out of bounds");
            }
        }
        else
        {
            throw new Exception("TwoDNativeArray dim1 out of bounds");
        }
    }

    public T[] GetArray(int x)
    {
        T[] array = new T[dim2];
        int offset = x * dim2;
        for (int y = 0; y < dim2; y++)
        {
            array[y] = oneDArray[offset + y];
        }
        return array;
    }

    public NativeArray<T> oneDArray;
    public int dim1;
    public int dim2;
    
}
