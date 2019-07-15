using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;

[RequireComponent(typeof(IssueDataDisplay))]
public class CopyMover : MonoBehaviour, IMixedRealityPointerHandler
{
    public GameObject copyObject;

    private GameObject copyInstance;
    private ManipulationHandler handlerOnCopy;

    private IssueDataDisplay localDataDisplay;

    private void Awake()
    {
        localDataDisplay = GetComponent<IssueDataDisplay>();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (!IssueSelectionManager.Instance.SelectionModeActive)
        {
            copyInstance = ResourceManager.Instance.NetworkInstantiate(copyObject, transform.position, transform.rotation);
            handlerOnCopy = copyInstance?.GetComponent<ManipulationHandler>();
            IssueDataDisplay remoteDataDisplay = copyInstance?.GetComponent<IssueDataDisplay>();
            if (handlerOnCopy == null || remoteDataDisplay == null)
            {
                if (handlerOnCopy == null)
                {
                    SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IMixedRealityPointerHandler), copyInstance);
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
        }
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        if (handlerOnCopy != null)
        {
            handlerOnCopy.gameObject.SetActive(true);
            handlerOnCopy.OnPointerDragged(eventData);
        }
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (handlerOnCopy != null)
        {
            handlerOnCopy.OnPointerUp(eventData);
        }
    }
}
