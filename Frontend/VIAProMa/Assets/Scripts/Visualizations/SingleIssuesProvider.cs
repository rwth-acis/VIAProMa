using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleIssuesProvider : IVisualizationContentProvider
{
    public event EventHandler ContentChanged;

    public List<Issue> Issues { get; private set; }

    public SingleIssuesProvider()
    {
        Issues = new List<Issue>();
    }

    public void SelectContent()
    {
        IssueSelectionManager.Instance.StartSelectionMode(Issues);
    }

    public void EndContentSelection()
    {
        Issues = IssueSelectionManager.Instance.EndSelectionMode();
        ContentChanged?.Invoke(this, EventArgs.Empty);
    }
}
