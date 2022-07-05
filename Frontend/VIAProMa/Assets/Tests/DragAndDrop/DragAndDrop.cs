using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.IssueSelection;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations;

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

    //--highlighting Stuff--
    bool issueIsGrabbed = false;

    //A list of Visualizations that currently overlap with the Issue
    List<GameObject> currentHits;

    //--Indicator Stuff--
    List<LineRenderer> overlapIndicators;
    [SerializeField]
    GameObject indicatorLine;
    int uniqueHitCounter = 0;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        currentHits = new List<GameObject>();
        overlapIndicators = new List<LineRenderer>();

        IssueManipulator = GetComponentInParent<IssueSelector>();
        if(IssueManipulator == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueSelector), gameObject);
        }

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

        //--highlighting Stuff--
        grabComponent.OnManipulationStarted.AddListener(SetManipulationStartedFlag);
        grabComponent.OnManipulationEnded.AddListener(SetManipulationEndedFlag);

        //Make it so object is not influenced by forces
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void Update()
    {
        //--Indicator Stuff--
        //only show lines if more than one visualization is hit
        if(uniqueHitCounter > 1)
        {
            for (int i = 0; i < currentHits.Count; i++)
            {
                //set the line end position and activate it
                Vector3 targetPos = currentHits[i].transform.position;
                LineRenderer laser = overlapIndicators[i];
                laser.SetPosition(1, laser.transform.worldToLocalMatrix * ((Vector4)targetPos + new Vector4(0, 0, 0, 1)));
                overlapIndicators[i].enabled = true;
            }
        }
        else if(uniqueHitCounter == 1)
        {
            //deactivate lines that are still pointing to the last visualization
            overlapIndicators.ForEach(x => x.enabled = false);
        }
    }

    #region TriggerEvents
    public void OnTriggerEnter(Collider potentialTarget)
    {
        if(issueIsGrabbed)
        {
            AddObjectToHitsList(potentialTarget.gameObject);
        }
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

        Debug.Log("Issue " + issueDataDisplay.Content.Name + " added to " + visualization.gameObject.name);//TODO remove
    }

    void RemoveObjectFromHitsList(GameObject target)
    {
        Visualization visualization = target.transform.parent.GetComponent<Visualization>();
        if (visualization == null)
        {
            return;
        }

        if(!currentHits.Contains(visualization.gameObject))
        {
            //necessary because a visualization being dragged on and then off an Issue would potentially result in problems
            return;
        }

        currentHits.Remove(visualization.gameObject);
        //--Indicator Stuff--
        uniqueHitCounter = (new HashSet<GameObject>(currentHits)).Count;

        //--Indicator Stuff--
        //remove a line from the list as it is not needed anymore
        Destroy(overlapIndicators[0].gameObject);
        overlapIndicators.RemoveAt(0);

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
        Visualization visualization = target.transform.parent.GetComponent<Visualization>();
        if (visualization == null)
        {
            return;
        }

        Debug.Log(target.gameObject.name + "; " + target.transform.parent.gameObject.name);

        currentHits.Add(visualization.gameObject);
        //--Indicator Stuff--
        uniqueHitCounter = (new HashSet<GameObject>(currentHits)).Count;

        //--Indicator Stuff--
        //add a line which points to one of the visualizations
        overlapIndicators.Add(Instantiate(indicatorLine, transform).GetComponent<LineRenderer>());

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

    //--highlighting Stuff--
    void SetManipulationStartedFlag(ManipulationEventData eventData)
    {
        issueIsGrabbed = true;
    }
    void SetManipulationEndedFlag(ManipulationEventData eventData)
    {
        issueIsGrabbed = false;
    }

    private void OnDestroy()
    {
        grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);
        //--highlighting Stuff--
        grabComponent.OnManipulationStarted.RemoveListener(SetManipulationStartedFlag);
        grabComponent.OnManipulationEnded.RemoveListener(SetManipulationEndedFlag);
    }
}
