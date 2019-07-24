using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : Visualization
{
    private IProgressBarVisuals progressBarVisuals;

    private string title;

    public string Title
    {
        get => title;
        set
        {
            title = value;
            progressBarVisuals.SetTitle(title);
        }
    }

    private void Awake()
    {
        progressBarVisuals = GetComponent<IProgressBarVisuals>();
        if (progressBarVisuals == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IProgressBarVisuals), gameObject);
        }

        ContentProvider = new SingleIssuesProvider();
        Title = "";
    }

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

    private static int[] CountStates(List<Issue> issues)
    {
        int[] states = new int[3];
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
