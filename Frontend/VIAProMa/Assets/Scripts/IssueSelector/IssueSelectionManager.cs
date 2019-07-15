using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssueSelectionManager : Singleton<IssueSelectionManager>
{
    public List<Issue> SelectedIssues { get; private set; }

    public event EventHandler SelectionModeChanged;
    public event EventHandler<IssueSelectionChangedArgs> IssueSelectionChanged;

    public bool SelectionModeActive
    {
        get;private set;
    }

    protected override void Awake()
    {
        base.Awake();
        SelectedIssues = new List<Issue>();
        SelectionModeActive = false;
    }

    public void StartSelectionMode()
    {
        StartSelectionMode(new List<Issue>());
    }

    public void StartSelectionMode(List<Issue> selectedIssues)
    {
        SelectedIssues = selectedIssues;
        SelectionModeActive = true;
        SelectionModeChanged?.Invoke(this, EventArgs.Empty);
    }

    public List<Issue> EndSelectionMode()
    {
        SelectionModeActive = false;
        SelectionModeChanged?.Invoke(this, EventArgs.Empty);
        return SelectedIssues;
    }

    public void SetSelected(Issue issue)
    {
        SelectedIssues.Add(issue);
        IssueSelectionChanged?.Invoke(this, new IssueSelectionChangedArgs(issue, true));
    }

    public void SetDeselected(Issue issue)
    {
        bool removeSuccessful = SelectedIssues.Remove(issue);
        if (removeSuccessful)
        {
            IssueSelectionChanged?.Invoke(this, new IssueSelectionChangedArgs(issue, false));
        }
    }

    public bool IsSelected(Issue issue)
    {
        return SelectedIssues.Contains(issue);
    }
}
