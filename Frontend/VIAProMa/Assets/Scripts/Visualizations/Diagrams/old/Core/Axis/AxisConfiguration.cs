using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes one possible configuration for the axis
/// </summary>
public class AxisConfiguration
{
    /// <summary>
    /// The gap in world units which should be between two labels
    /// </summary>
    private const float minimumGap = 0.1f;

    /// <summary>
    /// If true, labels have a horizontal orientation, otherwise vertical orientation is used
    /// </summary>
    public bool HorizontalTextOrientation { get; set; }

    /// <summary>
    /// The font size of the labels
    /// </summary>
    public int FontSize { get; set; }

    /// <summary>
    /// Labeling used in this configuration
    /// </summary>
    public List<string> Labels { get; set; }

    /// <summary>
    /// Creates an axis configuration object
    /// </summary>
    /// <param name="labels">The labels to use</param>
    /// <param name="fontSize">The font size which is used in this configuration</param>
    /// <param name="horizontalTextOrientation">If true, the labels are oriented horizontally</param>
    public AxisConfiguration(List<string> labels, int fontSize, bool horizontalTextOrientation)
    {
        FontSize = fontSize;
        HorizontalTextOrientation = horizontalTextOrientation;
        Labels = labels;
    }

    /// <summary>
    /// Scores the used format, e.g. decimal number (100) or scientific 1 * 10^2
    /// Note, that in this implementation it will always return 1 since no formatting is used
    /// </summary>
    /// <returns>A score for the formating (here always 1)</returns>
    private float FormatScore()
    {
        return 1; // changed since we do not want a factored representation
    }

    /// <summary>
    /// Scores the font size based on some minimum font size and a target font size
    /// </summary>
    /// <param name="minFontSize">Hard limit on how small the font may become</param>
    /// <param name="targetFontSize">Optimum font size which should be used if possible</param>
    /// <returns>A score on how good the font size is</returns>
    private float FontScore(int minFontSize, int targetFontSize)
    {
        if (FontSize >= targetFontSize) // best score for a font size which is bigger or equal to the ideal font size
        {
            return 1;
        }
        else if (FontSize < targetFontSize && FontSize >= minFontSize) // font size is larger than minimum but smaller than ideal => penalty for smaller fonts
        {
            return 0.2f * (FontSize - minFontSize + 1) / (targetFontSize - minFontSize);
        }
        else // worst score for font sizes smaller than the minimum font size
        {
            return float.MinValue;
        }
    }

    /// <summary>
    /// Calculates an orientation score for this axis configuration
    /// The score is between -infinity and 1 (in this case only -0.5 or 1)
    /// 1 is the best score
    /// </summary>
    /// <returns>The orientation score of this axis configuration</returns>
    private float OrientationScore()
    {
        // prefer horizontal text since it is more comfortable to read
        if (HorizontalTextOrientation)
        {
            return 1;
        }
        else
        {
            return -0.5f; // assign a small penalty for vertical text
        }
    }

    /// <summary>
    /// Calculates the overlap score of the axis configuration
    /// Score is between -infinity and 1
    /// 1 is the best score
    /// </summary>
    /// <param name="horizontalAxisOrientation">True if the axis is oriented horizontally</param>
    /// <param name="availableSpace">The available space of this axis' length in world units</param>
    /// <returns>The overlap score of the axis configuration</returns>
    private float OverlapScore(bool horizontalAxisOrientation, float availableSpace)
    {
        // get the minimum gap and judge it
        float labelsMinimumGap = GetMinimumGap(horizontalAxisOrientation, availableSpace);

        if (labelsMinimumGap < 0) // a configuration with overlapping labels gets the worst score
        {
            return float.MinValue;
        }
        else if (labelsMinimumGap > minimumGap) //  best score for a configuration where all gaps are bigger than the ideal minimum gap
        {
            return 1;
        }
        else // labels are not overlapping but smaller than the ideal minimum gap => assign a penalty based on how small the gap is
        {
            return 2f - minimumGap / labelsMinimumGap;
        }
    }

    /// <summary>
    /// Returns the minimum gap between labels in this configuration
    /// If this value is negative, it means that the labels overlaps
    /// </summary>
    /// <param name="horizontalAxisOrientation">True if the axis is oriented horizontally</param>
    /// <param name="availableSpace">The available space of the axis' length in world units</param>
    /// <returns>The minimum gap in world units between the labels of this axis configuration</returns>
    private float GetMinimumGap(bool horizontalAxisOrientation, float availableSpace)
    {
        // spaceBetweenTwoEntries is the distance between the midpoints of two labels
        // the labels are scattered uniformly across the axis
        float spaceBetweenTwoEntries = availableSpace / Labels.Count;

        Vector2 lastSize = Vector2.zero;

        float minGap = float.MaxValue;

        // go over all labels, calculate the gap to the previous label and return the minimum gap
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

    /// <summary>
    /// Calculates the legibility score of this axis configuration
    /// Score is between -infinity and 1
    /// 1 is the best score
    /// </summary>
    /// <param name="horizontalAxisOrientation">True if the axis should be oriented horizontally</param>
    /// <param name="availableSpace">The available space for the axis length in world units</param>
    /// <param name="minFontSize">The minimum font size</param>
    /// <param name="targetFontSize">The optimal font size which should be used if possible</param>
    /// <returns>The legibility score sof this axis configuration</returns>
    public float LegibilityScore(bool horizontalAxisOrientation, float availableSpace, int minFontSize, int targetFontSize)
    {
        // legibility score is the average of the format, font, orientation and overlap scores
        return (FormatScore() + FontScore(minFontSize, targetFontSize) 
            + OrientationScore() + OverlapScore(horizontalAxisOrientation, availableSpace)) / 4f;
    }

    /// <summary>
    /// Optimizes the legibility of the labels on the axis and returns the best configuration
    /// </summary>
    /// <param name="labels">The list of labels which should be placed on the axis</param>
    /// <param name="horizontalAxisOrientation">If set to true, the axis is oriented horizontally</param>
    /// <param name="possibilities">The possible configurations from which the function will choose the best one</param>
    /// <param name="availableSpace">Space in world units which is available for the entire axis length</param>
    /// <param name="minFontSize">Minimum font size</param>
    /// <param name="targetFontSize">Optimal font size which should be used if possible</param>
    /// <param name="bestScore">The legibility score of the best configuration</param>
    /// <returns></returns>
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

        // go over all possible configuraitons and get their score
        // return the best score
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

    /// <summary>
    /// Generates a list of all possible axis configurations based on the given labels
    /// </summary>
    /// <param name="labels">The labels which should be displayed on hte axis</param>
    /// <returns>The list of all possible axis configurations which have the given labels</returns>
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
