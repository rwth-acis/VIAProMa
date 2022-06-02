using i5.VIAProMa.ResourceManagagement;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    [RequireComponent(typeof(PaperDataDisplay))]

    public class PaperCopyMover : MonoBehaviour, IMixedRealityPointerHandler
    {
        [Tooltip("The prefab which should be instantiated as a copy")]
        public GameObject copyObject;

        private GameObject copyInstance;
        private ObjectManipulator handlerOnCopy;

        private PaperDataDisplay localDataDisplay;

        /// <summary>
        /// Sets the component up
        /// </summary>
        private void Awake()
        {
            localDataDisplay = GetComponent<PaperDataDisplay>();
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
            if (!PaperSelectionManager.Instance.SelectionModeActive
                //clicking the edit or delete button shouldn't spawn a card
                && currentPointerTarget.GetComponent<EditButton>() == null && currentPointerTarget.GetComponent<DeleteButton>() == null)
            {
                copyInstance = Instantiate(copyObject, this.transform.localPosition, transform.rotation);
                PaperDataDisplay remoteDataDisplay = copyInstance?.GetComponent<PaperDataDisplay>();
                remoteDataDisplay.Setup(localDataDisplay.Content);
                // create the copy, get the relevant components and set them up
                //ResourceManager.Instance.SceneNetworkInstantiate(copyObject, transform.position, transform.rotation,
                //    (obj) =>
                //    {
                //        copyInstance = obj;
                //        handlerOnCopy = copyInstance?.GetComponentInChildren<ObjectManipulator>();
                //        PaperDataDisplay remoteDataDisplay = copyInstance?.GetComponent<PaperDataDisplay>();
                //        if (handlerOnCopy == null || remoteDataDisplay == null)
                //        {
                //            if (handlerOnCopy == null)
                //            {
                //                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ObjectManipulator), copyInstance);
                //            }
                //            if (remoteDataDisplay == null)
                //            {
                //                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(PaperDataDisplay), copyInstance);
                //            }
                //            PhotonNetwork.Destroy(copyInstance);
                //        }
                //        else
                //        {
                //            remoteDataDisplay.Setup(localDataDisplay.Content);
                //            handlerOnCopy.OnPointerDown(eventData);
                //        }
                //    });
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
