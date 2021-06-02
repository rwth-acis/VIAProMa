using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using System;

public class IssueUpdater : MonoBehaviour
{

    private EditIssueMenu editMenu;
    private ProjectTracker projectTracker;
    private IssueDataDisplay issueDataDisplay;


    /// <summary>
    /// Called if the GameObject is enabled
    /// Registers for the IssueEdited and IssueDeleted Events
    /// </summary>
    private void OnEnable()
    {
        //Get the edit menu from the project tracker
        issueDataDisplay = GetComponent<IssueDataDisplay>();
        if (issueDataDisplay == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueDataDisplay), gameObject);
        }
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
        editMenu = projectTracker.editIssueMenu;
        if (editMenu != null)
        {
            editMenu.IssueEdited += OnIssueEdited;
        }
        if (projectTracker != null)
        {
            projectTracker.IssueDeleted += OnIssueDeleted;
        }
    }

    /// <summary>
    /// Called if the GameObject is disabled
    /// De-registers for the IssueEdited and IssueDeleted Events
    /// </summary>
    private void OnDisable()
    {
        if (editMenu != null)
        {
            editMenu.IssueEdited -= OnIssueEdited;
        }
        if (projectTracker != null)
        {
            projectTracker.IssueDeleted -= OnIssueDeleted;
        }
    }

    /// <summary>
    /// Called if the issue has been edited from the issue shelf
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnIssueDeleted(object sender, IssueDeletedArgs e)
    {
        if(e.IssueID == issueDataDisplay.Content.Id)
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Called if the issue has been edited from the issue shelf
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnIssueEdited(object sender, IssueEditedArgs e)
    {
        if(e.IssueID == issueDataDisplay.Content.Id)
        {
            Issue newIssue = new Issue(issueDataDisplay.Content.Source, issueDataDisplay.Content.Id, e.NewName, e.NewDescription, issueDataDisplay.Content.ProjectId, issueDataDisplay.Content.Creator, issueDataDisplay.Content.Status, issueDataDisplay.Content.CreationDateString, issueDataDisplay.Content.ClosedDateString, issueDataDisplay.Content.Developers, issueDataDisplay.Content.Commenters);
            issueDataDisplay.Setup(newIssue);
        }
    }

}
