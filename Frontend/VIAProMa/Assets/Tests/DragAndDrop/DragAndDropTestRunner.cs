using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.Visualizations.KanbanBoard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropTestRunner : MonoBehaviour
{
    public KanbanBoardColumn column;
    public int issues = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SingleIssuesProvider provider = new SingleIssuesProvider();
            for (int i = 0; i < issues; i++)
            {
                provider.Issues.Add(new Issue(DataSource.REQUIREMENTS_BAZAAR, 1, "Issue " + i, "Description for " + i, 1, new User(), IssueStatus.OPEN, "", "", new User[0], new User[0]));
            }
            column.ContentProvider = provider;
        }
    }
}
