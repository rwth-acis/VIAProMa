using UnityEngine;
using Org.Requirements_Bazaar.API;
using Org.Requirements_Bazaar.DataModel;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;

public class CreateIssueMenu : MonoBehaviour
{
    private ShelfConfigurationMenu configurationMenu;
    private IssuesLoader issueLoader;

    [SerializeField] private CreateIssueMenuOpener opener;
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;

    public void Start()
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
    }

    // Called when the CreateIssue button on theCreateIssue Window is pressed
    public async void CreateIssue()
    {
        ReqBazShelfConfiguration reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;

        Category category;
        category = await RequirementsBazaarManager.GetCategory(reqBazShelfConfiguration.SelectedCategory.id);
        Category[] categoryarray = new Category[1];
        categoryarray[0] = category;


        await RequirementsBazaarManager.CreateRequirement(reqBazShelfConfiguration.SelectedProject.id, issueName.text, issueDescription.text, categoryarray);

        issueLoader.LoadContent();
        opener.CloseMenu();
    }
}