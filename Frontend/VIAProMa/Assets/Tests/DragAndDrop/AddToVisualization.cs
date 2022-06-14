using i5.VIAProMa.UI;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.IssueSelection;

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
        if (onVisualizationNotIssue)
        {
            collisionChecker.RaycastHitEvent.AddListener(AddObjectToThis);
        }
        else
        {
            collisionChecker.RaycastHitEvent.AddListener(AddThisToObject);
        }

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
        IssueDataDisplay issueDataDisplay = target.GetComponentInChildren<IssueDataDisplay>();
        if(!issueDataDisplay)
        {
            return;
        }

        //just in case, test if this is a visualization
        Visualization visualization = GetComponent<Visualization>();
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
        IssueDataDisplay issueDataDisplay = GetComponentInChildren<IssueDataDisplay>();
        if (!issueDataDisplay)
        {
            Debug.LogError("Component needs to be added to an issue to use this function");
        }

        AddIssueToVisualization(visualization, issueDataDisplay);
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
            Debug.Log("Issue already in visualization.");
            return;
        }
        //done this way because just adding an element doesn't update the visualization
        issueList.Add(issueDataDisplay.Content);
        visualization.ContentProvider.Issues = issueList;
    }

    void RemoveObjectFromHitsList(GameObject target)
    {
        currentHits.Remove(target);

        //Deactivate selection indicator of the issue
        IssueSelector IssueManipulator;
        if(onVisualizationNotIssue)
        {
            IssueManipulator = target.GetComponentInChildren<IssueSelector>();
        }
        else
        {
            IssueManipulator = GetComponentInChildren<IssueSelector>();
        }

        IssueManipulator.Selected = false;
        IssueManipulator.UpdateViewIgnoreIssueSelectionManager();
    }

    void AddObjectToHitsList(GameObject target)
    {
        currentHits.Add(target);

        //Activate selection indicator of the issue
        IssueSelector IssueManipulator;
        if (onVisualizationNotIssue)
        {
            IssueManipulator = target.GetComponentInChildren<IssueSelector>();
        }
        else
        {
            IssueManipulator = GetComponentInChildren<IssueSelector>();
        }

        IssueManipulator.Selected = true;
        IssueManipulator.UpdateViewIgnoreIssueSelectionManager();
    }
}
