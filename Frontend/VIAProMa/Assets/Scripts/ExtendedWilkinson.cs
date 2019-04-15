using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExtendedWilkinson
{
    List<float> Q = new List<float>() { 1, 5, 2, 2.5f, 4, 3 };
    Vector4 weights = new Vector4(0.2f, 0.25f, 0.5f, 0.05f);

    public float Simplicity(float q, int j, float lmin, float lmax, float lstep)
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
            return float.MinValue; // worst score
        }

        float res = 1 - (i - 1) / (n - 1) - j + 1;

        return res;
    }

    public float Density(float r, float rt)
    {
        return 2f - Mathf.Max(r / rt, rt / r);
    }

    public float DensityMax(float r, float rt)
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

    public float Coverage(float dmin, float dmax, float lmin, float lmax)
    {
        float range = dmax - dmin;
        return 1f - 0.5f * (Mathf.Pow(dmax - lmax, 2) + Mathf.Pow(dmin - lmin, 2)) / Mathf.Pow(0.1f * range, 2);
    }

    public float CoverageMax(float dmin, float dmax, float span)
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

    public AxisConfiguration PerformExtendedWilkinson(float availableSpace, bool horizontalAxis, float targetDensity, float dataMin, float dataMax)
    {
        float bestScore = -2;
        AxisConfiguration bestOption = null;

        for (int j = 1; j < int.MaxValue; j++)
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
                            List<string> labels = stepSequence.Select(value => value.ToString()).ToList();

                            // optimize legibility
                            List<AxisConfiguration> possibilities = AxisConfiguration.GeneratePossibleConfigurations(labels);
                            float legibility;
                            bestOption = AxisConfiguration.OptimizeLegibility(labels, horizontalAxis, possibilities, availableSpace, 20, 80, out legibility);

                            float score = Vector4.Dot(new Vector4(s, c, d, legibility), weights);
                            if (score > bestScore)
                            {
                                bestScore = score;
                            }
                        }
                    }
                }
            }
        }
        return bestOption;
    }
}
