using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes one possible configuration for the axis
/// </summary>
public class AxisConfiguration
{
    private const float minimumGap = 0.1f;

    /// <summary>
    /// If true, labels have a horizontal orientation, otherwise vertical orientation is used
    /// </summary>
    public bool HorizontalTextOrientation { get; set; }

    /// <summary>
    /// The font size of the labels
    /// </summary>
    public int FontSize { get; set; }

    public List<string> Labels { get; set; }

    public AxisConfiguration(List<string> labels, int fontSize, bool horizontalTextOrientation)
    {
        FontSize = fontSize;
        HorizontalTextOrientation = horizontalTextOrientation;
        Labels = labels;
    }

    private float FormatScore()
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
        if (HorizontalTextOrientation)
        {
            return 1;
        }
        else
        {
            return -0.5f;
        }
    }

    private float OverlapScore(bool horizontalAxisOrientation, float availableSpace)
    {
        float labelsMinimumGap = GetMinimumGap(horizontalAxisOrientation, availableSpace);

        if (labelsMinimumGap < 0)
        {
            return float.MinValue;
        }
        else if (labelsMinimumGap > minimumGap)
        {
            return 1;
        }
        else
        {
            return 2f - minimumGap / labelsMinimumGap;
        }
    }

    private float GetMinimumGap(bool horizontalAxisOrientation, float availableSpace)
    {
        float spaceBetweenTwoEntries = availableSpace / Labels.Count;

        Vector2 lastSize = Vector2.zero;

        float minGap = float.MaxValue;

        for (int i = 0; i < Labels.Count; i++)
        {
            Vector2 size = TextSize.Instance.GetTextSize(Labels[i], FontSize);
            if (i > 0)
            {
                float gap;
                if (HorizontalTextOrientation == horizontalAxisOrientation)
                {
                    gap = spaceBetweenTwoEntries - size.x / 2 - lastSize.x / 2;
                }
                else
                {
                    gap = spaceBetweenTwoEntries - size.y / 2 - lastSize.y / 2;
                }

                if (gap < minimumGap)
                {
                    minGap = gap;
                }
            }

            lastSize = size;
        }

        return minGap;
    }

    public float LegibilityScore(bool horizontalAxisOrientation, float availableSpace, int minFontSize, int targetFontSize)
    {
        return (FormatScore() + FontScore(minFontSize, targetFontSize) 
            + OrientationScore() + OverlapScore(horizontalAxisOrientation, availableSpace)) / 4f;
    }

    public static AxisConfiguration OptimizeLegibility(
        List<string> labels,
        bool horizontalAxisOrientation,
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
            float score = possibilities[i].LegibilityScore(horizontalAxisOrientation, availableSpace, minFontSize, targetFontSize);
            if (score > bestScore)
            {
                best = possibilities[i];
                bestScore = score;
            }
        }

        return best;
    }

    public static List<AxisConfiguration> GeneratePossibleConfigurations(List<string> labels)
    {
        List<AxisConfiguration> possibilities = new List<AxisConfiguration>();
        for (int fontSize = 20; fontSize <= 100; fontSize += 5)
        {
            for (int i = 0; i < 2; i++)
            {
                AxisConfiguration conf = new AxisConfiguration(labels, fontSize, i == 0);
                possibilities.Add(conf);
            }
        }
        return possibilities;
    }
}
