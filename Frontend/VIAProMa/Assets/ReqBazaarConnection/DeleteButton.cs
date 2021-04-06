using TMPro;
using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using Org.Requirements_Bazaar.API;

public class DeleteButton : MonoBehaviour
{
    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;
    public TextMeshPro requirementName;

    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    // Called when the delete button on the issue bar is pressed
    public async void DeleteIssue()
    {
        await RequirementsBazaarManager.DeleteRequirement(requirementName.text,projectTracker.currentProjectID);
        issueLoader.LoadContent();
    }
}
