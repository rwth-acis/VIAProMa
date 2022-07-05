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

    //Dict to save coroutines combined with the gameObject they would add, so they can be stopped individually
    Dictionary<GameObject, Coroutine> startedRoutines;

    IssueSelector IssueManipulator;
    IssueDataDisplay issueDataDisplay;
    ObjectManipulator grabComponent;

    //A list of Visualizations that currently overlap with the Issue
    List<GameObject> currentHits;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        currentHits = new List<GameObject>();
        startedRoutines = new Dictionary<GameObject, Coroutine>();

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

        //Make it so object is not influenced by forces
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }

    #region TriggerEvents
    public void OnTriggerEnter(Collider potentialTarget)
    {
        //restart coroutine if it already exists in the list; happens if object is deactivated then activated again inside trigger
        if(startedRoutines.ContainsKey(potentialTarget.gameObject))
        {
            StopCoroutine(startedRoutines[potentialTarget.gameObject]);
            startedRoutines.Remove(potentialTarget.gameObject);
        }
        //AddObjectToHitsList(potentialTarget.gameObject);
        startedRoutines.Add(potentialTarget.gameObject, StartCoroutine(AddIssueWithTimeDelay(potentialTarget.gameObject)));
        //StartCoroutine(AddIssueWithTimeDelay(potentialTarget));
    }
    public void OnTriggerExit(Collider potentialTarget)
    {
        //In case the coroutine for adding is still running, i.e. the time delay was not yet reached, stop it
        StopCoroutine(startedRoutines[potentialTarget.gameObject]);
        startedRoutines.Remove(potentialTarget.gameObject);

        RemoveObjectFromHitsList(potentialTarget.gameObject);
    }
    #endregion TriggerEvents


    internal IEnumerator AddIssueWithTimeDelay(GameObject target)
    {
        yield return new WaitForSecondsRealtime(DropWaitTime);

        //don't add target if it was deactivated during the wait time
        if(target.activeSelf)
        {
            AddObjectToHitsList(target);
        }

    }

    /// <summary>
    /// Add issue to a visualization. Can only be used if this component is part of an issue.
    /// Is ignored if the issue is already inside the visualization
    /// </summary>
    /// <param name="target">the visualization this issue should be added to.</param>
    void AddIssueToVisualization(GameObject target)
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
        Visualization visualization = target.GetComponentInParent<Visualization>();
        if (visualization == null)
        {
            return;
        }

        //If object is not in the list, the drop waittime was not yet over so nothing needs to be done
        if(!currentHits.Contains(visualization.gameObject))
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
        //Debug.Log(transform.parent.name + ": " + visualization.gameObject.name + " added to the Hits list.");

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
