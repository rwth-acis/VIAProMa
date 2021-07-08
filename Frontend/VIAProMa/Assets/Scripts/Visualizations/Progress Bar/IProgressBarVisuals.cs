using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ProgressBars
{
    /// <summary>
    /// Describes the progress bar visuals
    /// They should be able to show the percentage which is done and which is in progress
    /// </summary>
    public interface IProgressBarVisuals : IVisualizationVisualController
    {
        /// <summary>
        /// Gets or sets the percentage of issues which is done
        /// </summary>
        float PercentageDone { get; set; }

        /// <summary>
        /// Gets or sets the percentage of issues which is in progress
        /// </summary>
        float PercentageInProgress { get; set; }
    }
}