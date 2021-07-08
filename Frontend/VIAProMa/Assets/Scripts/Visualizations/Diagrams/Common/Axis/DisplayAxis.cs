using i5.VIAProMa.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Diagrams.Common.Axes
{

    public class DisplayAxis : IDisplayAxis
    {
        public string Title { get => Axis.Title; }
        public List<string> Labels { get; private set; }
        public float FontSize { get; private set; }
        public bool HorizontalAlignment { get; private set; }
        public bool HorizontalAxis { get; private set; }
        public IAxis Axis { get; }

        public DisplayAxis(IAxis axis, List<string> labels, float fontSize, bool horizontalAlignment, bool horizontalAxis)
        {
            Axis = axis;
            Labels = labels;
            FontSize = fontSize;
            HorizontalAlignment = horizontalAlignment;
            HorizontalAxis = horizontalAxis;
        }

        public static IDisplayAxis FindBestLegibility(List<IDisplayAxis> possibilities, out float bestScore, float minFontSize, float targetFontSize, float distanceThreshold, float axisLength)
        {
            bestScore = float.MinValue;
            if (possibilities.Count == 0)
            {
                Debug.LogError("Do not optimize legibility over empty possibility array");
                return null;
            }
            int maxScoreIndex = 0;
            for (int i=0;i<possibilities.Count;i++)
            {
                float score = possibilities[i].ScoreLegibility(minFontSize, targetFontSize, distanceThreshold, axisLength);
                if (score > bestScore)
                {
                    bestScore = score;
                    maxScoreIndex = i;
                }
            }
            return possibilities[maxScoreIndex];
        }

        public float ScoreLegibility(float minFontSize, float targetFontSize, float distanceThreshold, float axisLength)
        {
            float res = (ScoreFontSize(minFontSize, targetFontSize) + ScoreOrientation() + ScoreOverlap(distanceThreshold, axisLength)) / 3f;
            return res;
        }

        private float ScoreOrientation()
        {
            if (HorizontalAlignment)
            {
                return 1f;
            }
            else
            {
                return -0.5f;
            }
        }

        private float ScoreFontSize(float minFontSize, float targetFontSize)
        {
            if (FontSize == targetFontSize)
            {
                return 1f;
            }
            else if (minFontSize <= FontSize && FontSize < targetFontSize)
            {
                return 0.2f * (FontSize - minFontSize + 1) / (targetFontSize - minFontSize);
            }
            else
            {
                return float.MinValue;
            }
        }

        private float ScoreOverlap(float distanceThreshold, float axisLength)
        {
            float cellSize = axisLength / Labels.Count;

            float minimumScore = 1; // score can be 1 at most, so initialize it with this upper bound
            for (int i=0;i<Labels.Count-1;i++)
            {
                Vector2 size1 = TextSize.Instance.GetTextSize(Labels[i], FontSize);
                Vector2 size2 = TextSize.Instance.GetTextSize(Labels[i+1], FontSize);
                float distance;
                if (HorizontalAxis)
                {
                    distance = cellSize - size1.x / 2f - size2.x / 2f;
                }
                else
                {
                    distance = cellSize - size1.y / 2f - size2.y / 2f;
                }

                float scoreForThisPair;
                if (distance >= distanceThreshold)
                {
                    scoreForThisPair = 1;
                }
                else if (distance > 0)
                {
                    scoreForThisPair = 2f - distanceThreshold / distance;
                }
                else // overlap; distance <= 0
                {
                    scoreForThisPair = float.MinValue;
                }
                minimumScore = Mathf.Min(scoreForThisPair, minimumScore);
            }
            return minimumScore;
        }
    }
}
