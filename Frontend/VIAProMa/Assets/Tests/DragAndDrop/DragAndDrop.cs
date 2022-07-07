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

    [SerializeField]
    [Tooltip("If the issue card is moved faster than this speed, it will not be added to the visualization when dropping")]
    float speedThreshhold = 0.2f;
    Vector3 oldPosition;
    //coroutine that tests if speed Threshhold is reached
    Coroutine speedConditionCoroutine;
    //list of objects that wait to be added when speed is below threshhold
    List<GameObject> hitWaitList;

    //A list of Visualizations that currently overlap with the Issue
    List<GameObject> currentHits;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        currentHits = new List<GameObject>();
        hitWaitList = new List<GameObject>();

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
        hitWaitList.Add(potentialTarget.gameObject);
        if (speedConditionCoroutine != null)
        {
            StopCoroutine(speedConditionCoroutine);
        }
        speedConditionCoroutine = StartCoroutine(TestForSpeedCondition());
    }
    public void OnTriggerExit(Collider potentialTarget)
    {
        //if no more objects are on the waiting list, the coroutine for the speed condition can be stopped
        hitWaitList.Remove(potentialTarget.gameObject);
        if (hitWaitList.Count == 0)
        {
            StopCoroutine(speedConditionCoroutine);
        }

        RemoveObjectFromHitsList(potentialTarget.gameObject);
    }
    #endregion TriggerEvents

    /// <summary>
    /// This coroutine tests if the speed threshhold was reached and then adds all objects that are on the HitWaitList.
    /// It only does so when necessary, i.e. when there is an overlap, to save recources.
    /// </summary>
    internal IEnumerator TestForSpeedCondition()
    {

        oldPosition = transform.position;
        float speed;
        Debug.Log("-----------------------------------------------------------------");
        do
        {
            yield return null;
            speed = (transform.position - oldPosition).magnitude / Time.deltaTime;
            oldPosition = transform.position;
            Debug.Log(speed);
        } while (speed > speedThreshhold);

        //when speed condition is satisfied, add all current
        foreach (GameObject target in hitWaitList)
        {
            //don't add target if it was deactivated during the wait time
            if (target.activeSelf)
            {
                AddObjectToHitsList(target);
            }
        }
        hitWaitList.Clear();

    }

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
