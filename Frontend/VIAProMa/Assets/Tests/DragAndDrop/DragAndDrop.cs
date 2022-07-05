using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.IssueSelection;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations;
using Photon.Pun;

using Microsoft.MixedReality.Toolkit.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script needs a rigit body for the Trigger function to work
[RequireComponent(typeof(Rigidbody))]
public class DragAndDrop : MonoBehaviour
{
    //TODO add timedelay functionality if user study says that that would be usefull
    [SerializeField]
    [Tooltip("Time the component waits before dropping component actually adds it")]
    float DropWaitTime = 0.2f;
    float timeWaitedForDrop = 0;

    IssueSelector IssueManipulator;
    IssueDataDisplay issueDataDisplay;
    ObjectManipulator grabComponent;
    GameObject issueGameObject;

    //A list of Visualizations that currently overlap with the Issue
    List<GameObject> currentHits;

    //indicates that the Issue is being destroyed right now; happens after it is added to a visualization
    bool IssueIsDestroyed = false;
    [SerializeField]
    [Tooltip("Time the component gets before it is destroyed after being added to a visualization")]
    float destroyTime = 0.3f;
    float timeOffset = 0;
    Vector3 destroyStartPosition;
    Vector3 destroyStartSize;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        currentHits = new List<GameObject>();

        IssueManipulator = GetComponentInParent<IssueSelector>();
        if(IssueManipulator == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueSelector), gameObject);
        }

        issueGameObject = IssueManipulator.gameObject;

        issueDataDisplay = GetComponentInParent<IssueDataDisplay>();
        if(issueDataDisplay == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueDataDisplay), gameObject);
            Debug.LogError("The DragAndDrop Component needs to be a child of an Issue object");
        }

        grabComponent = transform.parent.GetComponentInChildren<ObjectManipulator>();
        if(grabComponent == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ObjectManipulator), gameObject);
        }

        //Make it so object is not influenced by forces
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void Update()
    {
        //if the issue is set to be destroyed, move it towards the visualization it is added to
        if(IssueIsDestroyed)
        {
            //if visualizations were moved out of reach of the Issue it doesn't have a target to move to anymore
            if(currentHits.Count == 0)
            {
                Debug.LogWarning("Issue is in destroy routine, but there is no visualization in its reach anymore. " +
                    "Destroy it immediately.");
                //destroy component over the network. If that doesn't work, destroy it locally
                try{PhotonNetwork.Destroy(issueGameObject);}
                catch{Destroy(issueGameObject);}
            }

            timeOffset += Time.deltaTime * (1.0f / destroyTime);

            //move Issue towards visualization, such that it reaches it after destroyTime seconds
            Vector3 endPosition = currentHits[0].transform.position;
            issueGameObject.transform.position = Vector3.Lerp(destroyStartPosition, endPosition, timeOffset);

            //change the size so it shrinks while moving
            issueGameObject.transform.localScale = Vector3.Lerp(destroyStartSize, destroyStartSize * 0.1f, timeOffset);

            //destroy the Issue when it reaches its goal
            if (timeOffset >= 1)
            {
                //destroy component over the network. If that doesn't work, destroy it locally
                try { PhotonNetwork.Destroy(issueGameObject); }
                catch { Destroy(issueGameObject); }
            }
        }
    }

    #region TriggerEvents
    public void OnTriggerEnter(Collider potentialTarget)
    {
        AddObjectToHitsList(potentialTarget.gameObject);
    }
    public void OnTriggerExit(Collider potentialTarget)
    {
        RemoveObjectFromHitsList(potentialTarget.gameObject);
    }
    #endregion TriggerEvents

    /// <summary>
    /// Add issue to a visualization. Can only be used if this component is part of an issue.
    /// Is ignored if the issue is already inside the visualization
    /// </summary>
    /// <param name="target">the visualization this issue should be added to.</param>
    public void AddIssueToVisualization(GameObject target)
    {
        //test if target is a visualization
        Visualization visualization = target.GetComponent<Visualization>();

        List<Issue> issueList = new List<Issue>(visualization.ContentProvider.Issues);
        //only add an issue to a visualization once
        if (issueList.Contains(issueDataDisplay.Content))
        {
            Debug.Log("Issue " + issueDataDisplay.Content.Name + " already in visualization.");
            return;
        }
        //done this way because just adding an element doesn't update the visualization
        issueList.Add(issueDataDisplay.Content);
        visualization.ContentProvider.Issues = issueList;

        //set the start position and size of the destroy routine in the update function
        destroyStartPosition = issueGameObject.transform.position;
        destroyStartSize = issueGameObject.transform.localScale;
        IssueIsDestroyed = true;
    }

    void RemoveObjectFromHitsList(GameObject target)
    {
        //test if target is a visualization
        Visualization visualization = target.GetComponentInParent<Visualization>();
        if (visualization == null)
        {
            return;
        }

        currentHits.Remove(visualization.gameObject);

        //remove listener so letting go no longer adds this issue to visualizations
        grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);

        //Deactivate selection indicator of the issue, but only if it doesn't overlap with any visualization
        if (currentHits.Count > 0)
        {
            return;
        }

        IssueManipulator.Selected = false;
        //manually update view because the Selected variable only does so if IssueSelectionManager is in selection mode
        IssueManipulator.UpdateViewIgnoreIssueSelectionManager();
    }

    void AddObjectToHitsList(GameObject target)
    {
        //test if target is a visualization
        Visualization visualization = target.GetComponentInParent<Visualization>();
        if (visualization == null)
        {
            return;
        }

        currentHits.Add(visualization.gameObject);

        //Activate selection indicator of the issue
        IssueManipulator.Selected = true;
        IssueManipulator.UpdateViewIgnoreIssueSelectionManager();

        //Add listener so as soon as object is let go it is added to the target visualization
        grabComponent.OnManipulationEnded.AddListener(ManipulationEnded);
    }

    public void ManipulationEnded(ManipulationEventData eventData)
    {
        //When Issue is let go, add it to all visualizations it overlaps with
        foreach(GameObject vis in currentHits)
        {
            AddIssueToVisualization(vis);
        }
    }

    private void OnDestroy()
    {
        grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);
    }
}
