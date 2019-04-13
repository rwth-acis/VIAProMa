using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedWilkinson
{
    List<float> Q = new List<float>() { 1, 5, 2, 2.5f, 4, 3 };

    public float SimplicityMax(float q, int j)
    {
        int n = Q.Count;
        int i = Q.FindIndex((x) => { return x == q; });

        if (i != -1)
        {
            i += 1; // correct to array numbering in paper (starting at 1 and not 0)
        }
        else
        {
            Debug.LogError("Could not find q=" + q + " in list Q");
            return float.MinValue; // worst result
        }

        float res = 1 - (i - 1) / (n - 1) - j + 1;

        return res;
    }

    /// <summary>
    /// Computes the density measure for a number of labels
    /// </summary>
    /// <param name="k">Looped number of axis labels</param>
    /// <param name="m">Number of axis labels</param>
    /// <returns>The approximation for the density measure</returns>
    public float DensityMax(int k, int m)
    {
        if (k >= m)
        {
            return 2f - (k - 1f) / (m - 1f);
        }
        else
        {
            return 1f;
        }
    }
}
