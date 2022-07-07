using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Visualizations;
using UnityEngine;

namespace Tests.Visualizations.ListTaskCards
{
    public class ListTaskCardTestRunner : MonoBehaviour
    {
        private const int SimulatedIssuesCount = 1;
        [SerializeField] private Visualization[] visualizations;
        private int globalCount;

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F5)) return;
            foreach (var visualization in visualizations)
            {
                visualization.ContentProvider.SelectContent();

                for (var i = 0; i < SimulatedIssuesCount; i++)
                {
                    var issue = new Issue(DataSource.GITHUB, -1, "Test" + globalCount++, "A test" + globalCount, 0,
                        null, IssueStatus.CLOSED, "", "",
                        null, null);
                    visualization.ContentProvider.Issues.Add(issue);
                }

                visualization.ContentProvider.EndContentSelection();
            }
        }
    }
}