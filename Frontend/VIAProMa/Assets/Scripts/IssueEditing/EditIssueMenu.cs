using UnityEngine;
using Org.Requirements_Bazaar.API;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;

public class EditIssueMenu : MonoBehaviour
{
    [SerializeField] private TextMeshPro requirement_title;
    [SerializeField] private TextMeshPro requirement_description;
    [SerializeField] private TextMeshPro issue_title;
    [SerializeField] private TextMeshPro issue_description;

    [SerializeField] public GameObject requirementBazaar_UI;
    [SerializeField] public GameObject gitHub_UI;


    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;

    [HideInInspector] public TextMeshPro issueName;
    [HideInInspector] public TextMeshPro issueDescription;

    //Set references
    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    //Sets the default text to the current requirement information of the Requirement Bazaar UI
    public void SetText_RequirementBazaar()
    {
        requirement_title.text = issueName.text;
        requirement_description.text = issueDescription.text;
    }

    //Sets the default text to the current requirement information of the Requirement Bazaar UI
    public void SetText_GitHub()
    {
        issue_title.text = issueName.text;
        issue_description.text = issueDescription.text;
    }

    //Closes the issue edit window
    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Called when the confirm button on the issue edit window is pressed - Requirement Bazaar UI
    public async void EditRequirement()
    {
        await RequirementsBazaarManager.EditRequirement(issueName.text, projectTracker.currentProjectID, requirement_title.text, requirement_description.text);
        issueLoader.LoadContent();
        Close();
    }

    // Called when the confirm button on the issue edit window is pressed - GitHub UI
    public async void EditIssue()
    {
        //TODO Implement GitHubManager and EditIssue method
        //await GitHubManager.EditIssue(issueName.text, projectTracker.currentProjectID, issue_title.text, issue_description.text);
        issueLoader.LoadContent();
        Close();
    }
}
