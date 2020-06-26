using System;
using UnityEngine;

public class BezierCurve
{
    public static Vector3[] calculateCubicCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, int segmentCount)
    {
        Vector3[] curve = new Vector3[segmentCount];

        for (int i = 1; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 Vector3 = CalculateCubicBezierVector3(t, p1, p2, p3, p4);
            curve[i - 1] = Vector3;
        }

        return curve;
    }

    static Vector3 CalculateCubicBezierVector3(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public static Vector3[] calculateCurve(Vector3[] controlPoints, int segmentCount)
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
}
