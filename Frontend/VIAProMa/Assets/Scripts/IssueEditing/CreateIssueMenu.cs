using UnityEngine;
using Org.Git_Hub.API;
using Org.Requirements_Bazaar.API;
using Org.Requirements_Bazaar.DataModel;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;
using i5.VIAProMa.DataModel.API;

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

    // Called when the CreateIssue button on theCreateIssue Window is pressed
    public async void CreateIssue()
    {
        switch (configurationMenu.ShelfConfiguration.SelectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                Category category;
                if(projectTracker.currentCategory.id == 0)
                {
                    category = await RequirementsBazaarManager.GetCategory(1175);
                }
                else
                {
                    category = await RequirementsBazaarManager.GetCategory(projectTracker.currentCategory.id);
                }
                Category[] categoryarray = new Category[1];
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