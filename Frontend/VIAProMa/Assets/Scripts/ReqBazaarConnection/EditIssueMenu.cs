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

    //Set references
    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    //Sets the default text to the current requirement information
    public void SetText()
    {
        title.text = requirementName.text;
        description.text = requirementDescription.text;
    }

    //Closes the issue edit window
    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Called when the confirm button on the issue edit window is pressed
    public async void EditIssue()
    {
        await RequirementsBazaarManager.EditRequirement(requirementName.text, projectTracker.currentProjectID, title.text, description.text);
        issueLoader.LoadContent();
        Close();
    }
}
