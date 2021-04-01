using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Org.Requirements_Bazaar.API;
using Org.Requirements_Bazaar.DataModel;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.DataModel.API;
using System.Threading.Tasks;
using TMPro;

public class EditButtonsScript : MonoBehaviour
{
    private ShelfConfigurationMenu configurationMenu;
    [SerializeField] private TextMeshPro requirementName;

    public void Start()
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
    }

    // Called when the delete button on the issue bar is pressed
    public async void DeleteIssue()
    {
        ReqBazShelfConfiguration reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;

        await RequirementsBazaarManager.DeleteRequirement(requirementName.text, reqBazShelfConfiguration.SelectedProject.id);
        Destroy(transform.parent.gameObject);
    }

    // Called when the edit button on the issue bar is pressed
    public async void EditIssue()
    {
        ReqBazShelfConfiguration reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;

        await RequirementsBazaarManager.EditRequirement(requirementName.text, reqBazShelfConfiguration.SelectedProject.id);
    }
}
