using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager which handles the selection of issue cards
/// Singleton which should be placed in every scene on a manager GameObject
/// </summary>
public class IssueSelectionManager : Singleton<IssueSelectionManager>
{
    /// <summary>
    /// The list of selected issues
    /// </summary>
    public List<Issue> SelectedIssues { get; private set; }

    /// <summary>
    /// Event which is invoked if the selection mode is changed, i.e. if the selection mode is started or ended
    /// </summary>
    public event EventHandler SelectionModeChanged;
    /// <summary>
    /// Event which is invoked if an issue was selected or deselected
    /// The issue and it was selected or deselected can be found in the event arguments
    /// </summary>
    public event EventHandler<IssueSelectionChangedArgs> IssueSelectionChanged;

    /// <summary>
    /// True if the selection mode is currently active and the user can select issue cards
    /// </summary>
    public bool SelectionModeActive
    {
        get;private set;
    }

    /// <summary>
    /// Initializes the component
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        SelectedIssues = new List<Issue>();
        SelectionModeActive = false;
    }

    /// <summary>
    /// Starts the selection mode with an empty initial list of selected issues
    /// </summary>
    public void StartSelectionMode()
    {
        StartSelectionMode(new List<Issue>());
    }

    /// <summary>
    /// Starts the selection mode with the given issues as the initial list
    /// </summary>
    /// <param name="selectedIssues">The list of issues which are already selected</param>
    public void StartSelectionMode(List<Issue> selectedIssues)
    {
        SelectedIssues = selectedIssues;
        SelectionModeActive = true;
        SelectionModeChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Ends the selectoin mode
    /// </summary>
    /// <returns>The list of issues which were selected by the user</returns>
    public List<Issue> EndSelectionMode()
    {
        SelectionModeActive = false;
        SelectionModeChanged?.Invoke(this, EventArgs.Empty);
        return SelectedIssues;
    }

    /// <summary>
    /// Selects the given issue
    /// Selection Mode should be set to true; otherwise, the method has no effect
    /// </summary>
    /// <param name="issue">The issue to select</param>
    public void SetSelected(Issue issue)
    {
        if (SelectionModeActive)
        {
            SelectedIssues.Add(issue);
            IssueSelectionChanged?.Invoke(this, new IssueSelectionChangedArgs(issue, true));
        }
    }

    /// <summary>
    /// Deselects the given issue
    /// SelectionModeActive should be set to true; otherwise the method has no effect
    /// </summary>
    /// <param name="issue">The issue to deselect</param>
    public void SetDeselected(Issue issue)
    {
        if (SelectionModeActive)
        {
            bool removeSuccessful = SelectedIssues.Remove(issue);
            if (removeSuccessful)
            {
                IssueSelectionChanged?.Invoke(this, new IssueSelectionChangedArgs(issue, false));
            }
        }
    }

    /// <summary>
    /// Checks whether or not the given issue is selected
    /// </summary>
    /// <param name="issue">THe issue to check</param>
    /// <returns>True if the issue is selected; otherwise false</returns>
    public bool IsSelected(Issue issue)
    {
        return SelectedIssues.Contains(issue);
    }
}
