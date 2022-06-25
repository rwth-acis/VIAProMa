using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.KanbanBoard;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.ResourceManagagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDropTestRunner : MonoBehaviour
{
    public GameObject issuePrefab;
    public KanbanBoardColumn column;
    public int issues = 10;
    private int issueCount;

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F5))
        {
            //Fill the "Random Issues" Kanban Board with issues
            SingleIssuesProvider provider = new SingleIssuesProvider();
            for (int i = 0; i < issues; i++)
            {
                provider.Issues.Add(new Issue(DataSource.REQUIREMENTS_BAZAAR, i, "Issue " + i, "Description for " + i, 1, new User(), IssueStatus.OPEN, "", "", new User[0], new User[0]));
            }
            column.ContentProvider = provider;
        }

        
        if (Input.GetKeyDown(KeyCode.F6))
        {
            //Create a new Issue in the scene to test Drag And Drop
            GameObject IssueParent = GameObject.Find("Issues");
            GameObject newIssue = Instantiate(issuePrefab, transform.position, transform.rotation, IssueParent.transform);

            int id = Random.Range(issues + 1, issues + 1000);
            IssueStatus status = (IssueStatus)Random.Range(0, 3);
            newIssue.GetComponent<IssueDataDisplay>().Setup(
                new Issue(DataSource.REQUIREMENTS_BAZAAR, id, "Issue " + id, "Description for " + id, 1, new User(), status, "", "", new User[0], new User[0]));
        }



    }
}
