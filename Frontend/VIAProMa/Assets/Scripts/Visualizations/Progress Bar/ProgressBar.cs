using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract visualization logic of a progress bar
/// </summary>
public class ProgressBar : Visualization
{
    /// <summary>
    /// The progress bar is visualized using a dedicated progressBarVisuals script
    /// </summary>
    private IProgressBarVisuals progressBarVisuals;

    /// <summary>
    /// Initializes the component and checks its setup
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        progressBarVisuals = GetComponent<IProgressBarVisuals>();
        if (progressBarVisuals == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IProgressBarVisuals), gameObject);
        }

        ContentProvider = new SingleIssuesProvider();
        Title = "";
    }

    /// <summary>
    /// Updates the view of the visualization
    /// Determines the amount of issues which are done and in progress and converts it to percentage values
    /// The percentage values are handed to the visual controller
    /// </summary>
    public override void UpdateView()
    {
        if (ContentProvider.Issues.Count == 0)
        {
            progressBarVisuals.PercentageDone = 0f;
            progressBarVisuals.PercentageInProgress = 0f;
        }
        else
        {
            int[] states = CountStates(ContentProvider.Issues);
            progressBarVisuals.PercentageInProgress = ((float)states[1]) / ContentProvider.Issues.Count;
            progressBarVisuals.PercentageDone = ((float)states[2]) / ContentProvider.Issues.Count;
        }
        base.UpdateView();
    }

    /// <summary>
    /// For a given list of issues, this method determines the number of issues which are open, in progress or done
    /// </summary>
    /// <param name="issues">The list of issues to count</param>
    /// <returns>An integer array of length 3 where the first entry holds the number of open issues,
    /// the second entry stores the number of issues in progress and the third entry contains the number of done issues</returns>
    private static int[] CountStates(List<Issue> issues)
    {
        int[] states = new int[3];
        // for each issue check its state and increment the corresponding entry
        for (int i = 0; i < issues.Count; i++)
        {
            if (issues[i].Status == IssueStatus.OPEN)
            {
                states[0]++;
            }
            else if (issues[i].Status == IssueStatus.IN_PROGRESS)
            {
                states[1]++;
            }
            else
            {
                states[2]++;
            }
        }
        return states;
    }
}
