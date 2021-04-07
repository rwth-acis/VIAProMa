using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.DataModel.ReqBaz;

public class ProjectTracker : MonoBehaviour
{
    [Header("References")]
    private ShelfConfigurationMenu configurationMenu;
    private ReqBazShelfConfiguration reqBazShelfConfiguration;

    [Header("Tracked Parameters")]
    public int currentProjectID = 0;
    public Category currentCategory = null;
    public EditIssueMenu editIssueMenu = null;

    // Subscribe to events of project configuration
    public void Start()
    {
        editIssueMenu = GameObject.FindObjectOfType<EditIssueMenu>();
        editIssueMenu.gameObject.SetActive(false);
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;
    }


    /// <summary>
    /// Stores the current project ID
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        currentProjectID = reqBazShelfConfiguration.SelectedProject.id;
    }

    /// <summary>
    /// Stores the current category
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void CategoryChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)configurationMenu.ShelfConfiguration;
        currentCategory = reqBazShelfConfiguration.SelectedCategory;
    }


}
