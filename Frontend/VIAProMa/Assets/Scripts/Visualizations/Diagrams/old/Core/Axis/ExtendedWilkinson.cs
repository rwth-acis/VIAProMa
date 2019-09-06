using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the extended Wilkinson algorithm
/// It handles the numberings and labels on the axes on a diagram
/// </summary>
public static class ExtendedWilkinson
{
    /// <summary>
    /// prefrence-orderd list of nice step sizes
    /// </summary>
    static List<float> Q = new List<float>() { 1, 5, 2, 2.5f, 4, 3 };
    /// <summary>
    /// Weights the scores in order to get an overall score
    /// Order: simplicity, coverage, density, legibility
    /// </summary>
    static Vector4 weights = new Vector4(0.2f, 0.25f, 0.5f, 0.05f);

    /// <summary>
    /// Simplicity score which rates how simple the step range is
    /// Best score is 1
    /// </summary>
    /// <param name="q">The used step size constructor (should be an element of Q)</param>
    /// <param name="j">The skip amount; states how many elements from the sequence created from q should be skipped to create the final step size</param>
    /// <param name="lmin">Minimum value of the generated labels</param>
    /// <param name="lmax">Maximum value of the generated labels</param>
    /// <param name="lstep">Step size</param>
    /// <returns>Score which states how simple the chosen label number sequence is</returns>
    public static float Simplicity(float q, int j, float lmin, float lmax, float lstep)
    {
        int n = Q.Count;
        int i = Q.FindIndex((x) => { return x == q; });

        if (i != -1)
        {
            i += 1; // correct to array numbering in paper (starting at 1 and not 0)
        }
        else
        {
            Debug.LogError("Could not find q=" + q + "in list Q");
            return float.MinValue; // worst score
        }

        int v = 0;
        if ((Mathf.Floor(lmin % lstep) < 1e-6 || lstep - (Mathf.Floor(lmin % lstep)) < 1e-6) && lmin <= 0 && lmax >= 0)
        {
            v = 1; // indicate that the 0 will be used in the labeling
        }

        return 1f - (i - 1f) / (n - 1f) - j + v;
    }

    /// <summary>
    /// Approximation of the simplicity score
    /// Best score is 1
    /// </summary>
    /// <param name="q">The used step size constructor (should be an element of Q)</param>
    /// <param name="j">The skip amount; states how many elements from the sequence created from q should be skipped to create the final step size</param>
    /// <returns></returns>
    public static float SimplicityMax(float q, int j)
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
            return float.MinValue; // worst score
        }

        float res = 1 - (i - 1) / (n - 1) - j + 1;

        return res;
    }

    /// <summary>
    /// Determines how close a labeling's density is to the target density
    /// Best score is 1
    /// </summary>
    /// <param name="r">Density of the generated labeling</param>
    /// <param name="rt">Target density</param>
    /// <returns>The density score</returns>
    public static float Density(float r, float rt)
    {
        return 2f - Mathf.Max(r / rt, rt / r);
    }

    /// <summary>
    /// Approximation of the density score
    /// </summary>
    /// <param name="r">Density of the generated labeling</param>
    /// <param name="rt">Target density</param>
    /// <returns></returns>
    public static float DensityMax(float r, float rt)
    {
        if (r >= rt)
        {
            return 2 - r / rt;
        }
        else
        {
            return 1;
        }
    }

    /// <summary>
    /// Scores how well a data range is covered by the labeling
    /// Best score is 1
    /// </summary>
    /// <param name="dmin">The minimum of the data range</param>
    /// <param name="dmax">The maximum of the data range</param>
    /// <param name="lmin">The minimum of the label range</param>
    /// <param name="lmax">The maximum of the label range</param>
    /// <returns>A score which states how well a label sequence covers the data range on the axis</returns>
    public static float Coverage(float dmin, float dmax, float lmin, float lmax)
    {
        float range = dmax - dmin;
        return 1f - 0.5f * (Mathf.Pow(dmax - lmax, 2) + Mathf.Pow(dmin - lmin, 2)) / Mathf.Pow(0.1f * range, 2);
    }

    /// <summary>
    /// Approximation of coverage: states how well a data range is covered by the labeling
    /// Best score is 1
    /// </summary>
    /// <param name="dmin">The minimum of the data range</param>
    /// <param name="dmax">The maximum of the data range</param>
    /// <param name="span">The distance between the minimum and maximum on the axis</param>
    /// <returns>An approximation of a coverage score</returns>
    public static float CoverageMax(float dmin, float dmax, float span)
    {
        float range = dmax - dmin;
        if (span > range)
        {
            float half = (span - range) / 2f;
            return 1f - 0.5f * (Mathf.Pow(half, 2) + Mathf.Pow(half, 2)) / (Mathf.Pow(0.1f * range, 2));
        }
        else
        {
            return 1f;
        }
    }

    /// <summary>
    /// Performs the Extended Wilkinson algorithm which returns an optimal labeling for numeric numbers based on the given settings
    /// </summary>
    /// <param name="availableSpace">States how much space in world units there is available for the axis</param>
    /// <param name="horizontalAxis">If set to true, the axis is orientated horizontally</param>
    /// <param name="targetDensity">Specifies how many labels should be placed per world unit</param>
    /// <param name="dataMin">The minimum of the data range</param>
    /// <param name="dataMax">The maximum of the dtaa range</param>
    /// <param name="axisMin">Gives the chosen minimum of the axis labeling</param>
    /// <param name="axisMax">Gives the chosen maximum of the axis labeling</param>
    /// <returns>An axis configuration which includes styling and the chosen labels</returns>
    public static AxisConfiguration PerformExtendedWilkinson(float availableSpace, bool horizontalAxis, float targetDensity, float dataMin, float dataMax, out float axisMin, out float axisMax)
    {
        float bestScore = -2;
        axisMin = dataMin;
        axisMax = dataMax;
        AxisConfiguration bestOption = null;

        for (int j = 1; j < int.MaxValue; j++) // there are break statements which terminate this loop
        {
            foreach(float q in Q) // (j,q): determines step size
            {
                float sm = SimplicityMax(q, j);
                if (Vector4.Dot(new Vector4(sm, 1, 1, 1), weights) < bestScore)
                {
                    // finish the search
                    j = int.MaxValue - 1;
                    break;
                }
                for (int k = 2; k < int.MaxValue; k++) // try different numbers of labels
                {
                    float dm = DensityMax(k / availableSpace, targetDensity);
                    if (Vector4.Dot(new Vector4(sm, 1, dm, 1), weights) < bestScore)
                    {
                        break;
                    }
                    float delta = (dataMax - dataMin) / (k + 1) / (j * q);
                    for (int z = Mathf.CeilToInt(Mathf.Log10(delta)); z < int.MaxValue; z++) // power of 10 multiplier for the step size
                    {
                        float lStep = q * j * Mathf.Pow(10, z);
                        float cm = CoverageMax(dataMin, dataMax, lStep * (k - 1));
                        if (Vector4.Dot(new Vector4(sm, cm, dm, 1), weights) < bestScore)
                        {
                            break;
                        }
                        for (float start = Mathf.Floor(dataMax / lStep) - (k-1); start <= Mathf.Ceil(dataMin / lStep); start += 1f/j) // possible start labels
                        {
                            float lmin = start * lStep;
                            float lmax = lmin + (k - 1) * lStep;
                            float s = Simplicity(q, j, lmin, lmax, lStep);
                            float d = Density(k / availableSpace, targetDensity);
                            float c = Coverage(dataMin, dataMax, lmin, lmax);

                            if (Vector4.Dot(new Vector4(s, c, d, 1), weights) < bestScore)
                            {
                                continue;
                            }

                            List<float> stepSequence = Enumerable.Range(0, k).Select(x => lmin + x * lStep).ToList();
                            List<string> labels = stepSequence.Select(value => value.ToString("0.##")).ToList();

                            // optimize legibility
                            List<AxisConfiguration> possibilities = AxisConfiguration.GeneratePossibleConfigurations(labels);
                            float legibility;
                            bestOption = AxisConfiguration.OptimizeLegibility(labels, horizontalAxis, possibilities, availableSpace, 20, 100, out legibility);

                            float score = Vector4.Dot(new Vector4(s, c, d, legibility), weights);
                            if (score > bestScore)
                            {
                                bestScore = score;
                                axisMax = lmax;
                                axisMin = lmin;
                            }
                        }
                    }
                }
            }
        }
        return bestOption;
    }
}
