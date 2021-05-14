using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.DataModel.API;
using System;

public class ProjectTracker : MonoBehaviour
{
    [Header("References")]
    private ShelfConfigurationMenu configurationMenu;
    private ReqBazShelfConfiguration reqBazShelfConfiguration;
    private GitHubShelfConfiguration gitHubShelfConfiguration;

    [Header("Tracked Parameters")]
    public int currentProjectID = 0;
    public string currentRepositoryOwner;
    public string currentRepositoryName;
    public Category currentCategory = null;
    public EditIssueMenu editIssueMenu = null;
    public DataSource currentSource = DataSource.REQUIREMENTS_BAZAAR;

    /// <summary>
    /// Event which is invoked if an issue has been deleted
    /// </summary>
    public event EventHandler<IssueDeletedArgs> IssueDeleted;

    // Subscribe to events of project configuration
    public void Start()
    {
        editIssueMenu = GameObject.FindObjectOfType<EditIssueMenu>();
        editIssueMenu.gameObject.SetActive(false);
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().SourceChanged += SourceChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged_RequirementBazaar;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().GitHubOwnerChanged += OwnerChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().GitHubProjectChanged += ProjectChanged_GitHub;
    }
    public void OnlastDeletedChanged(string name, int projectID)
    {
        IssueDeletedArgs args = new IssueDeletedArgs(name, projectID);
        IssueDeleted?.Invoke(this, args);
    }
    /// <summary>
    /// Stores the current source
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void SourceChanged(object sender, System.EventArgs e)
    {
        configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
        currentSource = configurationMenu.ShelfConfiguration.SelectedSource;
    }

    /// <summary>
    /// Stores the current project ID
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged_RequirementBazaar(object sender, System.EventArgs e)
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

    /// <summary>
    /// Stores the current Repository Owner
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void OwnerChanged(object sender, System.EventArgs e)
    {
        if (currentSource == DataSource.GITHUB)
        {
            configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
            gitHubShelfConfiguration = (GitHubShelfConfiguration)configurationMenu.ShelfConfiguration;
            currentRepositoryOwner = gitHubShelfConfiguration.Owner;
        }
    }

    /// <summary>
    /// Stores the current Repository Name
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged_GitHub(object sender, System.EventArgs e)
    {
        if(currentSource == DataSource.GITHUB)
        {
            configurationMenu = GameObject.FindObjectOfType<ShelfConfigurationMenu>();
            gitHubShelfConfiguration = (GitHubShelfConfiguration)configurationMenu.ShelfConfiguration;
            currentRepositoryName = gitHubShelfConfiguration.RepositoryName;
        }
    }

}
