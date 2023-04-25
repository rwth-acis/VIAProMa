using UnityEngine;
using Org.Git_Hub.API;
using Org.Requirements_Bazaar.API;
using Org.Requirements_Bazaar.DataModel;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;
using i5.VIAProMa.DataModel.API;

/// <summary>
/// Contains the functionalities of the create button for the issue shelf
/// </summary>
public class CreateIssueMenu : MonoBehaviour
{
    private IssuesLoader issueLoader;
    private ProjectTracker projectTracker;

    [SerializeField] private ShelfConfigurationMenu configurationMenu;
    [SerializeField] private CreateIssueMenuOpener opener;
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;

    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    // Called when the CreateIssue button on the CreateIssue Window is pressed, creates an issue for the currently open project
    public async void CreateIssue()
    {
        switch (configurationMenu.ShelfConfiguration.SelectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                int category = 0;
                Project proj = await RequirementsBazaarManager.GetProject(projectTracker.currentProjectID);
                int[] categoryarray = null;
                if (projectTracker.currentCategory.id == 0)
                {

                    category = proj.DefaultCategoryId;
                }
                else
                {
                    if (projectTracker.currentCategory.id != 0)
                    {
                        category = projectTracker.currentCategory.id;
                    }
                }
                categoryarray = new int[1];
                categoryarray[0] = category;
                await RequirementsBazaarManager.CreateRequirement(projectTracker.currentProjectID, issueName.text, issueDescription.text, categoryarray);
                break;

            case DataSource.GITHUB:
                await GitHubManager.CreateIssue(projectTracker.currentRepositoryOwner,projectTracker.currentRepositoryName, issueName.text, issueDescription.text);
                break;
        }
        issueLoader.LoadContent();
        opener.CloseMenu();
    }
}