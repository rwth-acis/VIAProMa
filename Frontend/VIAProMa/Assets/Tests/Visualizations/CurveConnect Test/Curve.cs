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

    public static bool CurveCollsionCheck(Vector3[] curve, GameObject startBound, GameObject endBound)
    {
        for (int i = 0; i <= curve.Length - 2; i++)
        {
            Vector3 checkDirection = curve[i + 1] - curve[i];
            float checkLength = Vector3.Distance(curve[i], curve[i + 1]);
            checkDirection.Normalize();

            Vector3 center = curve[i] + checkDirection * checkLength / 2;

            if (collisonWithObstacle(center, new Vector3(0.1f, 0.1f, checkLength), Quaternion.LookRotation(checkDirection, new Vector3(0, 1, 0)), startBound, endBound))
            {
                return true;
            }
        }
        return false;
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
        curve[0] = goal;
        for (int i = 1; i < path.Count + 1; i++)
        {
            curve[i] = IntTriple.CellToVector(path[i - 1], stepSize);
        }
        curve[path.Count + 1] = start;
        return curve;
    }

    //Only things outside the boundingbox of start and goal are considered obstacles
    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends)
    {
        Collider[] collidionsWithStartGoal = Physics.OverlapBox(center, halfExtends, default, 1 << 6);

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        if (collidionsWithStartGoal.Length > 0)
        {
            return false;
        }
        else
        {
            Collider[] colliderInRange = Physics.OverlapBox(center, halfExtends);
            //Collides the cell with nothing else? 
            if (colliderInRange.Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    //TODO actual use orientation
    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends, Quaternion orientaton, GameObject startBound, GameObject endBound)
    {
        /*
        Collider[] collidionsWithStartGoal = Physics.OverlapBox(center, halfExtends, default, 1 << 6);

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        if (collidionsWithStartGoal.Length > 0)
        {
            return false;
        }
        else
        {
            Collider[] colliderInRange = Physics.OverlapBox(center, halfExtends);
            //Collides the cell with nothing else? 
            if (colliderInRange.Length == 0)
            {
                return false;
            }
        }
        return true;
        */
        return GetCollidorsFromObstacles(center, halfExtends, orientaton, startBound, endBound).Length != 0;
    }

    public static Collider[] GetCollidorsFromObstacles(Vector3 center, Vector3 halfExtends, Quaternion orientaton, GameObject startBound, GameObject endBound)
    {


        Collider[] potentialColliders = Physics.OverlapBox(center, halfExtends, orientaton);
        List<Collider> actuallColliders = new List<Collider>();

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        Collider startCollider = startBound.GetComponent<Collider>();
        Collider endCollider = endBound.GetComponent<Collider>();
        foreach (Collider collider in potentialColliders)
        {
            Vector3 startCorrVec = new Vector3();
            var test = startCollider.transform.position;
            float startCorrDist = 0;

            Vector3 endCorrVec = new Vector3();
            float endCorrDist = 0;

            bool isFromAppBar = false;
            if (collider.transform.root != null)
            {
                isFromAppBar = collider.transform.root.gameObject.name == "App Bar Configurable(Clone)";
            }

            if (!Physics.ComputePenetration(startCollider, startCollider.transform.position, startCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out startCorrVec, out startCorrDist)
                && !Physics.ComputePenetration(endCollider, endCollider.transform.position, endCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out endCorrVec, out endCorrDist)
                && !isFromAppBar)
            {
                actuallColliders.Add(collider);
            }
        }
        return actuallColliders.ToArray();
    }

}
