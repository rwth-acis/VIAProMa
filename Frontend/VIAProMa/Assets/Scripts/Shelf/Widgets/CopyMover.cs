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

namespace i5.VIAProMa.Shelves.Widgets
{
    /// <summary>
    /// If pressing down on an existing card with this component, a copy of the card will be created and the copy will be moved through this component
    /// Used on cards in the issue shelf
    /// Does not consume any of the input data since they are redirected to the created copy
    /// </summary>
    [RequireComponent(typeof(IssueDataDisplay))]
    public class CopyMover : MonoBehaviour, IMixedRealityPointerHandler
    {
        [Tooltip("The prefab which should be instantiated as a copy")]
        public GameObject copyObject;

        private GameObject copyInstance;
        private Microsoft.MixedReality.Toolkit.UI.ObjectManipulator handlerOnCopy;

        private IssueDataDisplay localDataDisplay;

        /// <summary>
        /// Sets the component up
        /// </summary>
        private void Awake()
        {
            localDataDisplay = GetComponent<IssueDataDisplay>();
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
            // only do this if we are out of selection mode, otherwise this is in conflict with the selection gesture
            if (!IssueSelectionManager.Instance.SelectionModeActive
                //clicking the edit or delete button shouldn't spawn a card
                && currentPointerTarget.GetComponent<EditButton>() == null && currentPointerTarget.GetComponent<DeleteButton>() == null)
            {
                // pass instantiation data to the copy so that other clients also know which issue is contained in the created copy
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
                        handlerOnCopy = copyInstance?.GetComponentInChildren<Microsoft.MixedReality.Toolkit.UI.ObjectManipulator>();
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
    }
}