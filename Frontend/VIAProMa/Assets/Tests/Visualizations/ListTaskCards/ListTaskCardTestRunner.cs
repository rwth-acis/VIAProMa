using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.Visualizations.Competence;
using UnityEngine;

public class ListTaskCardTestRunner : MonoBehaviour
{
    [Range(1, 10)] public int[] simulatedIssues;
    public Visualization visualizations;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.F5)) return;
        visualizations.ContentProvider.SelectContent();
        
        for (var i = 0; i < simulatedIssues.Length; i++)
        {
            var issue = new Issue(DataSource.GITHUB, -1, "Test", "A test", 0, null, IssueStatus.CLOSED, "", "",
                null, null);
            visualizations.ContentProvider.Issues.Add(issue);
        }

        visualizations.ContentProvider.EndContentSelection();
    }
}