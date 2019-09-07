using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDisplayAxis
{
    string Title { get; }
    List<string> Labels { get; }
    float FontSize { get; }
    bool HorizontalAlignment { get; }
    bool HorizontalAxis { get; }

    float ScoreLegibility(float minFontSize, float targetFontSize, float distanceThreshold, float axisLength);
}
