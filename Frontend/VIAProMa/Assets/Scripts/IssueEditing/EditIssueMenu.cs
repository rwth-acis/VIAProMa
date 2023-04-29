using UnityEngine;
using Org.Requirements_Bazaar.API;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;
using Org.Git_Hub.API;
using i5.VIAProMa.UI.InputFields;
using System;

/// <summary>
/// Manages the Edit-Window for Issue Cards allowing users to update title and description of issues
/// </summary>
public class EditIssueMenu : MonoBehaviour
{
    [SerializeField] private TextMeshPro requirement_title;
    [SerializeField] private TextMeshPro requirement_description;
    [SerializeField] private TextMeshPro issue_title;
    [SerializeField] private TextMeshPro issue_description;

    [SerializeField] public GameObject requirementBazaar_UI;
    [SerializeField] public GameObject gitHub_UI;

    // References to the project that is currently open
    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;

    [HideInInspector] public TextMeshPro issueName;
    [HideInInspector] public TextMeshPro issueDescription;
    [HideInInspector] public int issueID;
    public InputField inputField_Title_GitHub;
    public InputField inputField_Description_GitHub;
    public InputField inputField_Title_ReqBaz;
    public InputField inputField_Description_ReqBaz;

    /// <summary>
    /// Event which is invoked if an issue has been edited
    /// </summary>
    public event EventHandler<IssueEditedArgs> IssueEdited;

    /// <summary>
    /// Set references
    /// </summary>
    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    /// <summary>
    /// Sets the default text to the current requirement information of the Requirement Bazaar UI
    /// </summary>
    public void SetText_RequirementBazaar()
    {
        requirement_title.text = issueName.text;
        requirement_description.text = issueDescription.text;
        inputField_Title_ReqBaz.Text = issueName.text;
        inputField_Description_ReqBaz.Text = issueDescription.text;
    }

    /// <summary>
    /// Sets the default text to the current requirement information of the Requirement Bazaar UI
    /// </summary>
    public void SetText_GitHub()
    {
        issue_title.text = issueName.text;
        issue_description.text = issueDescription.text;
        inputField_Title_GitHub.Text = issueName.text;
        inputField_Description_GitHub.Text = issueDescription.text;
    }

    /// <summary>
    /// Closes the issue edit window
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the confirm button on the issue edit window is pressed, updates values - Requirement Bazaar UI
    /// </summary>
    public async void EditRequirement()
    {
        await RequirementsBazaarManager.EditRequirement(issueID, projectTracker.currentProjectID, requirement_title.text, requirement_description.text);
        issueLoader.LoadContent();
        IssueEditedArgs args = new IssueEditedArgs(issueID, requirement_title.text, requirement_description.text);
        IssueEdited?.Invoke(this, args);
        Close();
    }

    /// <summary>
    /// Called when the confirm button on the issue edit window is pressed, updates values - GitHub UI
    /// </summary>
    public async void EditIssue()
    {
        await GitHubManager.EditIssue(issueID, projectTracker.currentRepositoryOwner,projectTracker.currentRepositoryName, issue_title.text, issue_description.text);
        issueLoader.LoadContent();
        IssueEditedArgs args = new IssueEditedArgs(issueID, issue_title.text, issue_description.text);
        IssueEdited?.Invoke(this, args);
        Close();
    }
}
