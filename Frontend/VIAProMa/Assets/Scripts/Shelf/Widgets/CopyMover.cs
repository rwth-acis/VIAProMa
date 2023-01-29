using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.IssueSelection;
using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Anchoring;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using UnityEngine;
using System;
using System.Collections.Generic;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using VIAProMa.Assets.Scripts.Analytics;
using VIAProMa.Assets.Scripts.ProjectSettings;

namespace i5.VIAProMa.Shelves.Widgets
{
    /// <summary>
    /// If pressing down on an existing card with this component, a copy of the card will be created and the copy will be moved through this component
    /// Used on cards in the issue shelf
    /// Does not consume any of the input data since they are redirected to the created copy
    /// </summary>
    [RequireComponent(typeof(IssueDataDisplay))]
    public class CopyMover : AnalyticsObservibleComponent<LogpointLRSExportable>, IMixedRealityPointerHandler
    {
        [Tooltip("The prefab which should be instantiated as a copy")]
        public GameObject copyObject;

        private GameObject copyInstance;
        private ObjectManipulator handlerOnCopy;

        private IssueDataDisplay localDataDisplay;

        // A list of all the observers observing the state of this object for the analytics module.
        private List<IObserver<LogpointLRSExportable>> observers = new List<IObserver<LogpointLRSExportable>>();

        /// <summary>
        /// Sets the component up
        /// </summary>
        protected override void Awake()
        {
            localDataDisplay = GetComponent<IssueDataDisplay>();
            base.Awake();
        }

        /// <summary>
        /// Not used; required by the IMixedRealityPointerHandler interface
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
        }

        /// <summary>
        /// Called if the user starts a gesture on the object
        /// Creates a copy based on the given prefab and initializes the copy
        /// </summary>
        /// <param name="eventData">The event data of the gesture</param>
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            GameObject currentPointerTarget = eventData.Pointer.Result.CurrentPointerTarget;
            // Only do this if we are out of selection mode, otherwise this is in conflict with the selection gesture.
            if (!IssueSelectionManager.Instance.SelectionModeActive
                // Clicking the edit or delete button shouldn't spawn a card.
                && currentPointerTarget.GetComponent<EditButton>() == null && currentPointerTarget.GetComponent<DeleteButton>() == null)
            {
                // Notify the observers that the card has been clicked on.

                // Get meta data about the project (GitHub or Requirements Bazaar) the issue belongs to.
                ProjectTracker projectTracker = GameObject.FindObjectOfType<ProjectTracker>();

                // Generate the objectIRI. It is composed differently depending on the source of the issue.
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
                LogpointLRSExportable logpoint = new LogpointLRSExportable("http://id.tincanapi.com/verb/selected", "http://activitystrea.ms/schema/1.0/issue", objectIRI);
                NotifyObservers(logpoint);

                // Pass instantiation data to the copy so that other clients also know which issue is contained in the created copy.
                object[] instantiationData;
                if (localDataDisplay.Content.Source == DataSource.REQUIREMENTS_BAZAAR)
                {
                    instantiationData = new object[1];
                }
                else if (localDataDisplay.Content.Source == DataSource.GITHUB)
                {
                    instantiationData = new object[2];
                    instantiationData[1] = localDataDisplay.Content.ProjectId;
                }
                else
                {
                    Debug.LogError("Unexpected source: " + localDataDisplay.Content.Source, gameObject);
                    return;
                }

                instantiationData[0] = localDataDisplay.Content.Id; // same for ReqBaz and GitHub

                // create the copy, get the relevant components and set them up
                ResourceManager.Instance.SceneNetworkInstantiate(copyObject, transform.position, transform.rotation,
                    (obj) =>
                    {
                        copyInstance = obj;
                        handlerOnCopy = copyInstance?.GetComponentInChildren<ObjectManipulator>();
                        IssueDataDisplay remoteDataDisplay = copyInstance?.GetComponent<IssueDataDisplay>();
                        if (handlerOnCopy == null || remoteDataDisplay == null)
                        {
                            if (handlerOnCopy == null)
                            {
                                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ObjectManipulator), copyInstance);
                            }
                            if (remoteDataDisplay == null)
                            {
                                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueDataDisplay), copyInstance);
                            }
                            PhotonNetwork.Destroy(copyInstance);
                        }
                        else
                        {
                            remoteDataDisplay.Setup(localDataDisplay.Content);
                            handlerOnCopy.OnPointerDown(eventData);
                        }
                    },
                    instantiationData);
                HoloToolkit.Unity.Singleton<AnchorManager>.Instance.AttachToAnchor(copyInstance.gameObject);
            }
        }

        /// <summary>
        /// Called every frame during a drag gesture
        /// Shows the copy which was created in OnPointerDown and moves it
        /// </summary>
        /// <param name="eventData">The event data of the drag gesture</param>
        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // check if the copy was created and the handler was fetched
            // if yes, the instantiation went to plan
            if (handlerOnCopy != null)
            {
                handlerOnCopy.gameObject.SetActive(true);
                handlerOnCopy.OnPointerDragged(eventData);
            }
        }

        /// <summary>
        /// Called if the user ends a gesture on the object
        /// Redirects the event to the copy
        /// </summary>
        /// <param name="eventData">The event data of the gesture</param>
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // check if the copy was created and the handler was fetched
            // if yes, the instantiation went to plan
            if (handlerOnCopy != null)
            {
                handlerOnCopy.OnPointerUp(eventData);
            }
        }

        protected override void CreateObservers()
        {
            _ = new LRSBackendObserver(this);
        }
    }
}