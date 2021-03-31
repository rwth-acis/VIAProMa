using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Org.Requirements_Bazaar.API;
using Org.Requirements_Bazaar.DataModel;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.DataModel.API;
using System.Threading.Tasks;
using TMPro;

public class CreateIssueMenu : MonoBehaviour
{
    [SerializeField] private ShelfConfigurationMenu configurationMenu;
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;

    public void Start()
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Called when the createissue button on the createissue menu is pressed
    public async void CreateIssue()
    {
        ReqBazShelfConfiguration reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;

        Category category;
        category = await RequirementsBazaarManager.GetCategory(reqBazShelfConfiguration.SelectedCategory.id);
        Category[] categoryarray = new Category[1];
        categoryarray[0] = category;

        await RequirementsBazaarManager.CreateRequirement(reqBazShelfConfiguration.SelectedProject.id, issueName.text, issueDescription.text, categoryarray);

        Close();
    }
}