using System.Collections.Generic;
namespace i5.VIAProMa.Visualizations.Diagrams.Common.Axes
{
    public interface IDisplayAxis
    {
        string Title { get; }
        List<string> Labels { get; }
        float FontSize { get; }
        bool HorizontalAlignment { get; }
        bool HorizontalAxis { get; }

        float ScoreLegibility(float minFontSize, float targetFontSize, float distanceThreshold, float axisLength);
    }
}