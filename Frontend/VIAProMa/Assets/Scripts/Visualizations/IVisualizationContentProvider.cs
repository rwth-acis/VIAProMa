using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A visualization content provider can be used by a visualization to determine which issues should be used in the visualization
/// </summary>
public interface IVisualizationContentProvider
{
    /// <summary>
    /// The list of issues which define the content of the visualization
    /// </summary>
    List<Issue> Issues { get; set; }

    /// <summary>
    /// Event which is called if the issue list is changed
    /// </summary>
    event EventHandler ContentChanged;

    /// <summary>
    /// Starts the selection process in order to define the issues which belong to this content provider
    /// </summary>
    void SelectContent();

    /// <summary>
    /// Ends the selection mode and assigns the selected issues as content
    /// </summary>
    void EndContentSelection();
}
