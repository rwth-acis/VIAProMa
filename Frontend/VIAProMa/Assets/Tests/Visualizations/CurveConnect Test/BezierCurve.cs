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
            Vector3 point = CalculateCubicBezierPoint(t, p1, p2, p3, p4);
            curve[i - 1] = point;
        }

        return curve;
    }

    static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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
}
