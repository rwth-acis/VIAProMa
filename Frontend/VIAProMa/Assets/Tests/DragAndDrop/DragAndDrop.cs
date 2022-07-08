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

    //Is true iff the issue is currently being grabbed
    bool issueIsGrabbed = false;


    //A list of Visualizations that currently overlap with the Issue
    List<GameObject> currentHits;

    //list of all currently existing lines
    List<LineRenderer> overlapIndicators;
    [SerializeField]
    [Tooltip("Line Gameobject used to indicate which visualizations overlap with the Issue")]
    GameObject indicatorLine;
    //get unique visualizations that overlap with the issue; there can be duplicates in currentHits
    HashSet<GameObject> uniqueHitSet;

    //Awake is called when the script instance is being loaded
    void Awake()
    {
        currentHits = new List<GameObject>();

        hitWaitList = new List<GameObject>();

        overlapIndicators = new List<LineRenderer>();
        uniqueHitSet = new HashSet<GameObject>();


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
        else
        {
            grabComponent.OnManipulationStarted.AddListener(SetManipulationStartedFlag);
            grabComponent.OnManipulationEnded.AddListener(SetManipulationEndedFlag);
        }


        //Make it so object is not influenced by forces
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
    }
    private void Update()
    {
        //only show lines if more than one visualization is hit
        if (uniqueHitSet.Count > 1)
        {
            int i = 0;
            foreach(GameObject uniqueHit in uniqueHitSet)
            {
                //set the line end position and activate it
                Vector3 targetPos = uniqueHit.transform.position;
                LineRenderer laser = overlapIndicators[i];
                laser.SetPosition(1, laser.transform.worldToLocalMatrix * ((Vector4)targetPos + new Vector4(0, 0, 0, 1)));
                laser.enabled = true;
                i += 1;
            }
        }
        else if (uniqueHitSet.Count == 1)
        {
            //deactivate lines that are still pointing to the last visualization
            overlapIndicators.ForEach(x => x.enabled = false);
        }
    }

    #region TriggerEvents
    public void OnTriggerEnter(Collider potentialTarget)
    {

        //only add object if issue is grabbed to prevent it being highlighted if visualization is moved over it
        if (issueIsGrabbed)
        {
            hitWaitList.Add(potentialTarget.gameObject);
            if (speedConditionCoroutine != null)
            {
                StopCoroutine(speedConditionCoroutine);
            }
            speedConditionCoroutine = StartCoroutine(TestForSpeedCondition());
        }

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

        if (!currentHits.Contains(visualization.gameObject))
        {
            return;
        }

        currentHits.Remove(visualization.gameObject);
        //get count of unique overlapping visualizations, then get new list after removing visualization
        int oldHitCounter = uniqueHitSet.Count;
        uniqueHitSet = new HashSet<GameObject>(currentHits);

        if(uniqueHitSet.Count < oldHitCounter)
        {
            //remove a line from the list as it is not needed anymore
            Destroy(overlapIndicators[0].gameObject);
            overlapIndicators.RemoveAt(0);
        }


        //Deactivate selection indicator of the issue, but only if it doesn't overlap with any visualization
        if (currentHits.Count > 0)
        {
            return;
        }

        //remove listener so letting go no longer adds this issue to visualizations
        grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);

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

        //get count of unique overlapping visualizations, then get new list after removing visualization
        int oldHitCounter = uniqueHitSet.Count;
        uniqueHitSet = new HashSet<GameObject>(currentHits);

        if (uniqueHitSet.Count > oldHitCounter)
        {
            //add a line which is used to point to one of the visualizations
            overlapIndicators.Add(Instantiate(indicatorLine, transform).GetComponent<LineRenderer>());
        }

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

    #region GrabbingEvents
    void SetManipulationStartedFlag(ManipulationEventData eventData)
    {
        issueIsGrabbed = true;
    }
    void SetManipulationEndedFlag(ManipulationEventData eventData)
    {
        issueIsGrabbed = false;
    }
    #endregion GrabbingEvents

    private void OnDestroy()
    {
        //remove listeners to prevent possible memory leaks
        grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);
        grabComponent.OnManipulationStarted.RemoveListener(SetManipulationStartedFlag);
        grabComponent.OnManipulationEnded.RemoveListener(SetManipulationEndedFlag);
    }
}
