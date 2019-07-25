using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arguments of the issue selection changed event
/// It describes which issue was either selected or deselected
/// </summary>
public class IssueSelectionChangedArgs : EventArgs
{
    /// <summary>
    /// Creates the issue selection changed arguments
    /// </summary>
    /// <param name="issue">The issue which was selected or deselected</param>
    /// <param name="selected">True if the issue was selected, false if deselected</param>
    public IssueSelectionChangedArgs(Issue issue, bool selected)
    {
        ChangedIssue = issue;
        Selected = selected;
    }

    /// <summary>
    /// The issue for which the selection status was changed
    /// </summary>
    public Issue ChangedIssue { get; private set; }

    /// <summary>
    /// Indicates if the issue was selected (true) or deselected (false)
    /// </summary>
    public bool Selected { get; private set; }
}
