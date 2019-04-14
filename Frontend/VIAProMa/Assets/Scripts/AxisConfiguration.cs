using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes one possible configuration for the axis
/// </summary>
public class AxisConfiguration
{
    /// <summary>
    /// If true, labels have a horizontal orientation, otherwise vertical orientation is used
    /// </summary>
    public bool HorizontalOrientation { get; set; }

    /// <summary>
    /// The font size of the labels
    /// </summary>
    public int FontSize { get; set; }

    private float FormatScore(List<string> labels)
    {
        return 1; // changed since we do not want a factored representation
    }

    private float FontScore(int minFontSize, int targetFontSize)
    {
        if (FontSize >= targetFontSize)
        {
            return 1;
        }
        else if (FontSize < targetFontSize && FontSize >= minFontSize)
        {
            return 0.2f * (FontSize - minFontSize + 1) / (targetFontSize - minFontSize);
        }
        else
        {
            return float.MinValue;
        }
    }

    private float OrientationScore()
    {
        if (HorizontalOrientation)
        {
            return 1;
        }
        else
        {
            return -0.5f;
        }
    }

    private float OverlapScore(float availableSpace)
    {
        // TODO: implement
        return 1;
    }

    public float LegibilityScore(List<string> labels, float availableSpace, int minFontSize, int targetFontSize)
    {
        return (FormatScore(labels) + FontScore(minFontSize, targetFontSize) + OrientationScore() + OverlapScore(availableSpace)) / 4f;
    }

    public static AxisConfiguration OptimizeLegibility(
        List<string> labels,
        List<AxisConfiguration> possibilities,
        float availableSpace,
        int minFontSize,
        int targetFontSize,
        out float bestScore)
    {
        AxisConfiguration best = null;
        bestScore = float.MinValue;

        for (int i = 0; i < possibilities.Count; i++)
        {
            float score = possibilities[i].LegibilityScore(labels, availableSpace, minFontSize, targetFontSize);
            if (score > bestScore)
            {
                best = possibilities[i];
                bestScore = score;
            }
        }

        return best;
    }
}
