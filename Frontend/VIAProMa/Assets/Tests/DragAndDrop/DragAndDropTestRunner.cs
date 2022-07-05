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
    public GameObject issueNormalPrefab;
    public KanbanBoardColumn column;
    public int issues = 10;
    private int issueCount;

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

        if (Input.GetKeyDown(KeyCode.F6))
        {
            SpawnIssueInScene(true);
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            SpawnIssueInScene(false);
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            GameObject newIssue = Instantiate(issueNormalPrefab, transform.position, transform.rotation);

            int id = Random.Range(issues + 1, issues + 1000);
            IssueStatus status = (IssueStatus)Random.Range(0, 3);
            newIssue.GetComponent<IssueDataDisplay>().Setup(
                new Issue(DataSource.REQUIREMENTS_BAZAAR, 1, "Issue " + id, "Description for " + id, 1, new User(), status, "", "", new User[0], new User[0]));
        }
    }

    void SpawnIssueInScene(bool useRay)
    {
        GameObject newIssue = Instantiate(issuePrefab, transform.position, transform.rotation);

        int id = Random.Range(issues + 1, issues + 1000);
        IssueStatus status = (IssueStatus)Random.Range(0, 3);
        newIssue.GetComponent<IssueDataDisplay>().Setup(
            new Issue(DataSource.REQUIREMENTS_BAZAAR, 1, "Issue " + id, "Description for " + id, 1, new User(), status, "", "", new User[0], new User[0]));

        newIssue.GetComponent<CheckForCollision>().isInRayMode = useRay;
    }
}