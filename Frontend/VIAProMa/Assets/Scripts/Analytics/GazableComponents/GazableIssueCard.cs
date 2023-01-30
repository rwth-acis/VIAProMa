using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.DataModel.API;
using UnityEngine;

namespace VIAProMa.Assets.Scripts.Analytics.GazableComponents
{
    [RequireComponent(typeof(IssueDataDisplay))]
    public class GazableIssueCard : AnalyticsObservableComponent<LogpointLRSExportable>
    {
        public void GazeDetected()
        {
            // Get meta data about the project (GitHub or Requirements Bazaar) the issue belongs to.
            IssueDataDisplay localDataDisplay = GetComponent<IssueDataDisplay>();
            string objectIRI = "";
            if (localDataDisplay.Content.Source == DataSource.GITHUB)
            {
                objectIRI = string.Format("https://api.github.com/repositories/{0}/issues/{1}", localDataDisplay.Content.ProjectId, localDataDisplay.Content.Id);
            }
            else if (localDataDisplay.Content.Source == DataSource.REQUIREMENTS_BAZAAR)
            {
                string projectID = localDataDisplay.Content.ProjectId.ToString();
                objectIRI = string.Format("https://requirements-bazaar.org/projects/{0}/requirements/{1}", projectID, localDataDisplay.Content.Id);
            }
            else
            {
                // Initialize the IRI of the object in the LRS statement to an unknown source. Will be overwritten unless the DataSource of the issue is neither GITHUB nor REQUIREMENTS_BAZAAR.
                objectIRI = "Unknown Issue source!";
                Debug.LogError("Unexpected source: " + localDataDisplay.Content.Source);
            }
            LogpointLRSExportable logpoint = new LogpointLRSExportable("http://activitystrea.ms/schema/1.0/watch", "http://activitystrea.ms/schema/1.0/issue", objectIRI);
            NotifyObservers(logpoint);
        }

        protected override void CreateObservers()
        {
            // Logging LRS-Logpoints to the VIAProMA backend.
            _ = new LRSBackendObserver(this);

            // Logging LRS-Logpoints to LRS.
            _ = new LRSObserver(this);
        }
    }
}
