using System.Collections.Generic;
using UnityEngine;
using System;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.DataModel.API;

namespace VIAProMa.Assets.Scripts.Analytics.GazableComponents
{
    [RequireComponent(typeof(IssueDataDisplay))]
    public class GazableIssueCard : AnalyticsObservibleComponent<LogpointLRSExportable>
    {
        public void GazeDetected()
        {
            // Get meta data about the project (GitHub or Requirements Bazaar) the issue belongs to.
            ProjectTracker projectTracker = GameObject.FindObjectOfType<ProjectTracker>();

            IssueDataDisplay localDataDisplay = GetComponent<IssueDataDisplay>();
            string objectIRI = "";
            if (localDataDisplay.Content.Source == DataSource.GITHUB)
            {
                string repository = projectTracker.currentRepositoryName;
                string repositoryOwner = projectTracker.currentRepositoryOwner;
                objectIRI = string.Format("https://github.com/{0}/{1}/issues/{2}", repositoryOwner, repository, localDataDisplay.Content.Id);
            }
            else if (localDataDisplay.Content.Source == DataSource.REQUIREMENTS_BAZAAR)
            {
                string projectID = projectTracker.currentProjectID.ToString();
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
            //Logging LRS-Logpoints to the VIAProMA backend
            _ = new LRSBackendObserver(this);

            //Logging LRS-Logpoints to LRS
            _ = new LRSObserver(this);
        }
    }
}
