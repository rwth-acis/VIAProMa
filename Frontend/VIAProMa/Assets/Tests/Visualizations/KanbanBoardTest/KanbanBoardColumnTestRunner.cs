using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardColumnTestRunner : MonoBehaviour
{
    public KanbanBoardColumn kanbanBoardColumn;
    public int numberOfIssues = 10;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SingleIssuesProvider provider = new SingleIssuesProvider();
            for (int i=0;i<numberOfIssues;i++)
            {
                provider.Issues.Add(new Issue(DataSource.REQUIREMENTS_BAZAAR, 1, "Issue " + i, "Description for " + i, 1, new User(), IssueStatus.OPEN, "", "", new User[0], new User[0]));
            }
            kanbanBoardColumn.ContentProvider = provider;
        }
    }
}
