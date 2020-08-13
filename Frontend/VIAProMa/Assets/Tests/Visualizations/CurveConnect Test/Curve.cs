using System;
using UnityEngine;
using System.Collections.Generic;

public abstract class CurveGenerator
{ 
    public static Vector3[] CalculateBezierCurve(Vector3[] controlPoints, int segmentCount)
    {
        Vector3[] points = new Vector3[segmentCount + 1];
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            points[i] = CalculateBezierPoint(controlPoints,t);
        }
        return points;
    }

    static Vector3 CalculateBezierPoint(Vector3[] controlPoints, float t)
    {
        Vector3 result = Vector3.zero;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            result += (float)(BinomCoefficient(controlPoints.Length-1, i) * Math.Pow(1 - t, controlPoints.Length-1 - i) * Math.Pow(t, i)) * controlPoints[i];
        }
        return result;
    }

    public static long BinomCoefficient(long n, long k)
    {
        if (k > n) { return 0; }
        if (n == k) { return 1; } // only one way to chose when n == k
        if (k > n - k) { k = n - k; } // Everything is symmetric around n-k, so it is quicker to iterate over a smaller k than a larger one.
        long c = 1;
        for (long i = 1; i <= k; i++)
        {
            c *= n--;
            c /= i;
        }
        return c;
    }

    public static bool CurveCollsionCheck(Vector3[] curve, GameObject startObject, GameObject goalObject, int layermask = 0b1111111, bool checkEndCollision = true , float distanceToObstacle = 0.2f)
    {
        int curveIncrement = (curve.Length - 2) / 60;
        if (curveIncrement < 1)
            curveIncrement = 1;
        int lastChecked = 0;

        //Check from start to the first. This has to be done seperatly, because otherwise the sphere around start would detect collisions behind the object.
        Collider[] potentialColliders = Physics.OverlapCapsule(curve[0] + (curve[curveIncrement] - curve[0]).normalized * distanceToObstacle, curve[curveIncrement], distanceToObstacle, layermask);
        foreach (Collider coll in potentialColliders)
        {
            if (IsValidCollider(coll, startObject, goalObject))
                return true;
        }

        for (int i = curveIncrement; i < curve.Length - 1 - curveIncrement; i += curveIncrement)
        {
            potentialColliders = Physics.OverlapCapsule(curve[i], curve[i+curveIncrement],distanceToObstacle, layermask);
            foreach (Collider coll in potentialColliders)
            {
                if (IsValidCollider(coll, startObject, goalObject))
                    return true;
            }
            lastChecked = i + curveIncrement;
        }

        //Check form the last checked to the end
        potentialColliders = Physics.OverlapCapsule(curve[lastChecked] , curve[curve.Length - 1] +(curve[lastChecked] - curve[curve.Length - 1]).normalized * distanceToObstacle, distanceToObstacle, layermask);
        foreach (Collider coll in potentialColliders)
        {
            if (IsValidCollider(coll, startObject, goalObject))
                return true;
        }

        return false;
    }

    public static void CollTest(Vector3[] curve)
    {
        int curveIncrement = (curve.Length - 2) / 10;
        if (curveIncrement < 1)
            curveIncrement = 1;

        //Start to the first one
        Vector3 checkDirection1 = curve[1] - curve[0];
        float checkLength1 = checkDirection1.magnitude;
        checkDirection1.Normalize();
        GameObject test1 = new GameObject();
        CapsuleCollider coll1 = test1.AddComponent<CapsuleCollider>();
        coll1.radius = 0.3f;
        coll1.height = checkLength1;
        coll1.transform.rotation = Quaternion.LookRotation(checkDirection1, Vector3.up);
        coll1.transform.position = curve[0] + checkDirection1 * checkLength1 / 2;
        coll1.direction = 2;
        int lastChecked = 0;
        for (int i = 1; i < curve.Length - 1 - curveIncrement; i+= curveIncrement)
        {
            Vector3 checkDirection = curve[i + curveIncrement] - curve[i];
            float checkLength = checkDirection.magnitude;
            checkDirection.Normalize();
            GameObject test = new GameObject();
            CapsuleCollider coll = test.AddComponent<CapsuleCollider>();
            coll.radius = 0.3f;
            coll.height = checkLength+2*coll.radius;
            coll.transform.rotation = Quaternion.LookRotation(checkDirection, Vector3.up);
            coll.transform.position = curve[i] + checkDirection * checkLength / 2;
            coll.direction = 2;
            lastChecked = i + curveIncrement;
        }
        
    }

    public static int CurveCollsionCount(Vector3[] curve, GameObject startBound, GameObject endBound)
    {
        int collisionCount = 0;
        for (int i = 0; i <= curve.Length - 2; i++)
        {
            Vector3 checkDirection = curve[i + 1] - curve[i];
            float checkLength = Vector3.Distance(curve[i], curve[i + 1]);
            checkDirection.Normalize();

            Vector3 center = curve[i] + checkDirection * checkLength / 2;

            if (collisonWithObstacle(center, new Vector3(0.2f, 0.2f, checkLength), Quaternion.LookRotation(checkDirection, new Vector3(0, 1, 0)), startBound, endBound))
            {
                collisionCount++;
            }
        }
        return collisionCount;
    }

    public static float MaximalCurveAngel(Vector3[] curve)
    {
        float maximumAngel = float.MinValue;
        for (int i = 0; i <= curve.Length - 2; i++)
        {
            maximumAngel = Mathf.Max(maximumAngel, Vector3.Angle(curve[i],curve[i+1]));    
        }
        return maximumAngel;
    }

    public static float CurveLength(Vector3[] path)
    {
        float length = 0;
        for (int i = 0; i <= path.Length - 2; i++)
        {
            length += Vector3.Distance(path[i], path[i + 1]);
        }
        return length;
    }

    public static Vector3[] IntTripleArrayToCurve(List<IntTriple> path, Vector3 start, Vector3 goal, float stepSize)
    {
        Vector3[] curve = new Vector3[path.Count + 2];
        curve[0] = start;
        for (int i = 1; i < path.Count + 1; i++)
        {
            curve[i] = IntTriple.CellToVector(path[i - 1], stepSize);
        }
        curve[path.Count + 1] = goal;
        return curve;
    }

    //Only things outside the boundingbox of start and goal are considered obstacles
    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends, GameObject startObject, GameObject goalObject)
    {
        return collisonWithObstacle(center, halfExtends, Quaternion.identity, startObject, goalObject);
    }

    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends, Quaternion orientaton, GameObject startObject, GameObject goalObject, int layermask = 0b11111111, bool checkEndCollision = true)
    {
        Collider[] potentialColliders = Physics.OverlapBox(center, halfExtends, orientaton, layermask);
        List<Collider> actuallColliders = new List<Collider>();

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        foreach (Collider collider in potentialColliders)
        {
            if (IsValidCollider(collider, startObject, goalObject, checkEndCollision))
            {
                return true;
            }
         }
        return false;
    }


    public static Collider[] GetCollidorsFromObstacles(Vector3 center, Vector3 halfExtends, Quaternion orientaton, GameObject startObject, GameObject goalObject, int layermask = 0b11111111, bool checkEndCollision = true)
    {
        Collider[] potentialColliders = Physics.OverlapBox(center, halfExtends, orientaton, layermask);
        List<Collider> actuallColliders = new List<Collider>();

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        foreach (Collider collider in potentialColliders)
        {
            if (IsValidCollider(collider, startObject, goalObject))
            {
                actuallColliders.Add(collider);
            }
        }
        return actuallColliders.ToArray();
    }

    static private bool IsValidCollider(Collider collider, GameObject startObject, GameObject goalObject, bool checkEndCollision = true)
    {
        GameObject parent = collider.transform.root.gameObject;
        if (parent != startObject.transform.root.gameObject && parent != goalObject.transform.root.gameObject && (parent.name.Length < 8 || parent.name.Substring(0,7) != "App Bar"))
        {
            bool collidesWithStart = false;
            bool collidesWithGoal = false;

            if (checkEndCollision)
            {

                if (collider is BoxCollider || collider is SphereCollider || collider is CapsuleCollider)
                {
                    collidesWithStart = (collider.ClosestPoint(startObject.transform.position) - startObject.transform.position).magnitude < 0.1;

                    if (!collidesWithStart)
                    {
                        collidesWithGoal = (collider.ClosestPoint(goalObject.transform.position) - goalObject.transform.position).magnitude < 0.1;
                    }

                    if (!collidesWithStart && !collidesWithGoal)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
                return true;

           
        }
        return false;
    }

}
