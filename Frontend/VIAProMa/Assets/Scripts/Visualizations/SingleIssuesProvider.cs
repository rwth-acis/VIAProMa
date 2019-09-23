using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visualization Content Provider which collects single issues
/// </summary>
public class SingleIssuesProvider : IVisualizationContentProvider
{
    /// <summary>
    /// Invoked if the content of this visualization provider was changed
    /// </summary>
    public event EventHandler ContentChanged;

    private List<Issue> issues;

    /// <summary>
    /// The list of issues which should be included in the visualization
    /// </summary>
    public List<Issue> Issues
    {
        get => issues;
        set
        {
            issues = value;
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Initializies the issue list
    /// </summary>
    public SingleIssuesProvider()
    {
        Issues = new List<Issue>();
    }

    /// <summary>
    /// Starts the global selection mode
    /// After calling this function, issues can be selected
    /// </summary>
    public void SelectContent()
    {
        IssueSelectionManager.Instance.StartSelectionMode(Issues);
    }

    /// <summary>
    /// Ends the global selection mode
    /// Updates the issue list based on the selected issues
    /// Invokes the ContentChanged event
    /// </summary>
    public void EndContentSelection()
    {
        Issues = IssueSelectionManager.Instance.EndSelectionMode();
        ContentChanged?.Invoke(this, EventArgs.Empty);
    }
}
