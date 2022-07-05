using i5.VIAProMa.UI;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.IssueSelection;

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script uses the CheckForCollision Script to test for overlaps
[RequireComponent(typeof(CheckForCollision))]
public class AddToVisualization : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Set to true, if this component was added to a visualization, false if added to an issue")]
    bool onVisualizationNotIssue;

    [SerializeField]
    [Tooltip("Time the component waits before actually adding the component")]
    float DropWaitTime = 0.2f;

    float timeWaitedForDrop = 0;

    //either a list of Issues or a list of Visualizations, depending on onVisualizationNotIssue
    List<GameObject> currentHits;

    // Start is called before the first frame update
    void Awake()
    {
        currentHits = new List<GameObject>();
        CheckForCollision collisionChecker = GetComponent<CheckForCollision>();
        if (collisionChecker == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(CheckForCollision), gameObject);
            return;
        }
        //add Events to the collision checking class; TODO remove this as it is only for testing purposes
        //if (onVisualizationNotIssue)
        //{
        //    collisionChecker.RaycastHitEvent.AddListener(AddObjectToThis);
        //}
        //else
        //{
        //    collisionChecker.RaycastHitEvent.AddListener(AddThisToObject);
        //}

        collisionChecker.RaycastHitEvent.AddListener(AddObjectToHitsList);
        collisionChecker.RaycastHitStopEvent.AddListener(RemoveObjectFromHitsList);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject hit in currentHits)
        {
            //TODO test if person let the issue go
        }
        
    }

    /// <summary>
    /// Add an issue to a visualization. Can only be used if this component is part of a visualization.
    /// </summary>
    /// <param name="target">the game object that holds the issue.</param>
    public void AddObjectToThis(GameObject target)
    {
        //test if target is an issue
        IssueDataDisplay issueDataDisplay = target.GetComponentInParent<IssueDataDisplay>();
        if(!issueDataDisplay)
        {
            return;
        }

        //just in case, test if this is a visualization
        Visualization visualization = GetComponentInParent<Visualization>();
        if(!visualization)
        {
            Debug.LogError("Component needs to be added to a visualization to use this function");
            return;
        }

        AddIssueToVisualization(visualization, issueDataDisplay);
    }

    /// <summary>
    /// Add an issue to a visualization. Can only be used if this component is part of an issue.
    /// </summary>
    /// <param name="target">the visualization this issue should be added to.</param>
    public void AddThisToObject(GameObject target)
    {
        //test if target is a visualization
        Visualization visualization = target.GetComponentInParent<Visualization>();
        if (!visualization)
        {
            return;
        }

        //just in case, test if this is an issue
        IssueDataDisplay issueDataDisplay = GetComponentInParent<IssueDataDisplay>();
        if (!issueDataDisplay)
        {
            Debug.LogError("Component needs to be added to an issue to use this function");
        }
        AddIssueToVisualization(visualization, issueDataDisplay);

        Debug.Log(name + " added to " + target.transform.parent.name);
    }

    /// <summary>
    /// Add an issue to a visualization. Ignored, if the issue is already inside the visualization.
    /// </summary>
    /// <param name="visualization">Visualization component of the visualization.</param>
    /// <param name="issueDataDisplay">IssueDataDisplay Component of the issue.</param>
    void AddIssueToVisualization(Visualization visualization, IssueDataDisplay issueDataDisplay)
    {
        List<Issue> issueList = new List<Issue>(visualization.ContentProvider.Issues);
        //only add an issue once to a visualization
        if (issueList.Contains(issueDataDisplay.Content))
        {
            Debug.Log("Issue " + issueDataDisplay.Content.Name + " already in visualization.");
            return;
        }
        //done this way because just adding an element doesn't update the visualization
        issueList.Add(issueDataDisplay.Content);
        visualization.ContentProvider.Issues = issueList;
    }

    void RemoveObjectFromHitsList(GameObject target)
    {
        currentHits.Remove(target);

        //if (currentHits.Count > 0 && !onVisualizationNotIssue)
        //{
        //    return;
        //}

        //Deactivate selection indicator of the issue
        IssueSelector IssueManipulator;
        if(onVisualizationNotIssue)
        {
            IssueManipulator = target.GetComponentInParent<IssueSelector>();

            //necessary to ignore colliders that are no direct children of the uppermost parent
            string parentObjectName = target.transform.parent.name;
            if (parentObjectName == "Status" || parentObjectName == "Creator" || parentObjectName == "Source")
            {
                Debug.Log("Hit component that it ignores.");
                return;
            }

            ObjectManipulator grabComponent = target.transform.parent.GetComponentInChildren<ObjectManipulator>();
            //if there is no component, then there was obviously no listener added, so none has to be removed
            if(grabComponent)
            {
                grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);
            }
        }
        else
        {
            IssueManipulator = GetComponentInParent<IssueSelector>();
            ObjectManipulator grabComponent =  transform.parent.GetComponentInChildren<ObjectManipulator>();
            grabComponent.OnManipulationEnded.RemoveListener(ManipulationEnded);
        }
        if(IssueManipulator)
        {
            IssueManipulator.Selected = false;
            IssueManipulator.UpdateViewIgnoreIssueSelectionManager();
        }
    }

    void AddObjectToHitsList(GameObject target)
    {
        //currentHits.Add(target);
        Debug.Log(transform.parent.name + " hit: (" + target.name + ") " + target.transform.parent.name);

        //Activate selection indicator of the issue
        IssueSelector IssueManipulator;
        if (onVisualizationNotIssue)
        {
            IssueManipulator = target.GetComponentInParent<IssueSelector>();

            //test if target is an issue
            if (target.GetComponentInParent<IssueDataDisplay>())
            {
                //necessary to ignore colliders that are no direct children of the uppermost parent
                string parentObjectName = target.transform.parent.name;
                if (parentObjectName == "Status" || parentObjectName == "Creator" || parentObjectName == "Source")
                {
                    Debug.Log("Hit component that it ignores.");
                    return;
                }
                currentHits.Add(target);
                IssueManipulator.Selected = true;

                ObjectManipulator grabComponent = target.transform.parent.GetComponentInChildren<ObjectManipulator>();
                grabComponent.OnManipulationEnded.AddListener(ManipulationEnded);
            }
        }
        else
        {
            IssueManipulator = GetComponentInParent<IssueSelector>();

            //test if target is a visualization
            if (target.GetComponentInParent<Visualization>())
            {
                currentHits.Add(target);
                IssueManipulator.Selected = true;
                ObjectManipulator grabComponent = transform.parent.GetComponentInChildren<ObjectManipulator>();
                grabComponent.OnManipulationEnded.AddListener(ManipulationEnded);
            }

        }

        if(IssueManipulator)
        {
            IssueManipulator.UpdateViewIgnoreIssueSelectionManager();
        }
    }

    public void ManipulationEnded(ManipulationEventData eventData)
    {
        if (onVisualizationNotIssue)
        {
            AddObjectToThis(currentHits[0]);
        }
        else
        {
            AddThisToObject(currentHits[0]);
        }
    }

    private void OnDestroy()
    {
        CheckForCollision collisionChecker = GetComponent<CheckForCollision>();
        collisionChecker.RaycastHitEvent.RemoveListener(AddObjectToHitsList);
        collisionChecker.RaycastHitStopEvent.RemoveListener(RemoveObjectFromHitsList);
    }
}
