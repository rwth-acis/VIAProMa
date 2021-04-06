using UnityEngine;
using Org.Requirements_Bazaar.API;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;

public class EditIssueMenu : MonoBehaviour
{
    [SerializeField] private TextMeshPro title;
    [SerializeField] private TextMeshPro description;

    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;
    [HideInInspector] public TextMeshPro requirementName;
    [HideInInspector] public TextMeshPro requirementDescription;

    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
        title.text = requirementName.text;
        description.text = requirementDescription.text;
    }

    // Called when the edit button on the issue bar is pressed
    public async void EditIssue()
    {
        await RequirementsBazaarManager.EditRequirement(requirementName.text, projectTracker.currentProjectID, title.text, description.text);
        issueLoader.LoadContent();
    }
}
