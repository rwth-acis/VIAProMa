using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : Visualization
{
    private IProgressBarVisuals progressBarVisuals;

    private void Awake()
    {
        progressBarVisuals = GetComponent<IProgressBarVisuals>();
        if (progressBarVisuals == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IProgressBarVisuals), gameObject);
        }
    }

    public override void UpdateView()
    {
        int[] states = CountStates(ContentProvider.Issues);
        progressBarVisuals.PercentageInProgress = ((float)states[1]) / ContentProvider.Issues.Count;
        progressBarVisuals.PercentageDone = ((float)states[2]) / ContentProvider.Issues.Count;
        base.UpdateView();
    }

    private static int[] CountStates(List<Issue> issues)
    {
        int[] states = new int[3];
        for (int i=0;i<issues.Count;i++)
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
