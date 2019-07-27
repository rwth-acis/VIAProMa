using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProgressBarVisuals : IVisualizationVisualController
{
    float PercentageDone { get; set; }
    float PercentageInProgress { get; set; }
}
