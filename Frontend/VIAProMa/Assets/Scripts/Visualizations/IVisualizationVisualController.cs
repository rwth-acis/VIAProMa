using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Specifies a visual controller of an visualization
/// The visual controller's task is to arrange the GameObjects in a way that they represent the visualization data which are stored in the accomanying visualizaton component
/// </summary>
public interface IVisualizationVisualController
{
    /// <summary>
    /// The title of the visualization
    /// </summary>
    string Title {get; set;}
}
