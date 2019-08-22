using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfConfigurationMenu : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] private IssuesLoader shelf;

    [Header("UI Elements")]
    [SerializeField] private StringDropdownMenu sourceSelection;
    [SerializeField] private GameObject reqBazDisplays;
    [SerializeField] private GameObject gitHubDisplays;
    [SerializeField] private InputField reqBazProjectInput;
    [SerializeField] private CategoryDropdownMenu reqBazCategoryDropdownMenu;
    [SerializeField] private InputField gitHubOwnerInput;
    [SerializeField] private InputField gitHubRepositoryInput;

    private Project[] projects;
    private Category[] categories;

    public event EventHandler WindowClosed;

    private bool isConfiguring = true;

    public IShelfConfiguration ShelfConfiguration { get; private set; }

    public bool WindowEnabled
    { // not needed for configuration window => does not have an effect
        get; set;
    }

    private void Awake()
    {
        if (shelf == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(shelf));
        }
        if (sourceSelection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(sourceSelection));
        }
        if (reqBazDisplays == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(reqBazDisplays));
        }
        if (gitHubDisplays == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(gitHubDisplays));
        }
        if (reqBazProjectInput == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(reqBazProjectInput));
        }
        if (reqBazCategoryDropdownMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(reqBazCategoryDropdownMenu));
        }
        if (gitHubOwnerInput == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(gitHubOwnerInput));
        }
        if (gitHubRepositoryInput == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(gitHubRepositoryInput));
        }

        // populate the source dropdown menu with the available data sources
        List<StringData> sources = new List<StringData>();
        foreach (DataSource source in Enum.GetValues(typeof(DataSource)))
        {
            sources.Add(new StringData(source.GetDescription()));
        }
        sourceSelection.Items = sources;

        sourceSelection.ItemSelected += SourceSelected;
        reqBazProjectInput.TextChanged += ReqBazProjectInputFinished;
        reqBazCategoryDropdownMenu.ItemSelected += ReqBazCategorySelected;
        gitHubOwnerInput.TextChanged += GitHubOwnerInputFinished;
        gitHubRepositoryInput.TextChanged += GitHubRepositoryInputFinished;
    }

    private void Start()
    {
        SetDataSource(DataSource.REQUIREMENTS_BAZAAR); // first entry of dropdown box is Requirements Bazaar, so set this as the default source

        reqBazProjectInput.Text = "";
        gitHubOwnerInput.Text = "";
        gitHubRepositoryInput.Text = "";
        isConfiguring = false;
    }

    private void SourceSelected(object sender, EventArgs e)
    {
        DataSource selectedSource = (DataSource)sourceSelection.SelectedItemIndex;
        SetDataSource(selectedSource);
    }

    private void SetDataSource(DataSource selectedSource)
    {
        switch (selectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                // if the project was already set
                if (projects != null && projects.Length > 0)
                {
                    // if a category was already loaded
                    if (categories != null && categories.Length > 0)
                    {
                        ShelfConfiguration = new ReqBazShelfConfiguration(GetReqBazProject(reqBazProjectInput.Text), categories[reqBazCategoryDropdownMenu.SelectedItemIndex]);
                    }
                    else
                    {
                        ShelfConfiguration = new ReqBazShelfConfiguration(GetReqBazProject(reqBazProjectInput.Text));
                        LoadReqBazCategoryList();
                    }
                }
                else // nothing previously set
                {
                    ShelfConfiguration = new ReqBazShelfConfiguration();
                    LoadReqBazProjectList();
                }
                break;
            case DataSource.GITHUB:
                ShelfConfiguration = new GitHubShelfConfiguration(gitHubOwnerInput.Text, gitHubRepositoryInput.Text);
                break;
        }
        ShowControlsForSource();
        shelf.LoadContent();
    }

    private void ShowControlsForSource()
    {
        reqBazDisplays.SetActive(ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR);
        gitHubDisplays.SetActive(ShelfConfiguration.SelectedSource == DataSource.GITHUB);
    }

    private void ReqBazProjectInputFinished(object sender, EventArgs e)
    {
        if (isConfiguring)
        {
            return;
        }
        Debug.Log("Project input finished");
        Debug.Log(ShelfConfiguration.SelectedSource);
        Debug.Log(reqBazProjectInput.Text);
        if (ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR || string.IsNullOrEmpty(reqBazProjectInput.Text))
        {
            return;
        }
        // if the projects list is not loaded at this point, try again
        if (projects == null)
        {
            LoadReqBazProjectList();
        }
        // if the project list could still not be loaded => abort
        if (projects == null)
        {
            shelf.MessageBadge.ShowMessage(0);
            return;
        }
        Project selectedProject = GetReqBazProject(reqBazProjectInput.Text);
        ShelfConfiguration = new ReqBazShelfConfiguration(selectedProject);
        if (selectedProject == null) // project was not found
        {
            Debug.LogWarning("Project not found");
        }
        else // fetch categories
        {
            LoadReqBazCategoryList();
            shelf.ResetPage();
            shelf.LoadContent();
        }
    }

    private void ReqBazCategorySelected(object sender, EventArgs e)
    {
        if (isConfiguring)
        {
            return;
        }
        Category selectedCategory = categories[reqBazCategoryDropdownMenu.SelectedItemIndex];
        if (ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR)
        {
            return;
        }
        ((ReqBazShelfConfiguration)ShelfConfiguration).SelectedCategory = selectedCategory;
        shelf.ResetPage();
        shelf.LoadContent();
    }

    private void GitHubOwnerInputFinished(object sender, EventArgs e)
    {
        if (isConfiguring)
        {
            return;
        }
        ShelfConfiguration = new GitHubShelfConfiguration(gitHubOwnerInput.Text);
        // the owner was changed, so the repository is also different
        gitHubRepositoryInput.Text = "";
        shelf.ResetPage();
        shelf.LoadContent();
    }

    private void GitHubRepositoryInputFinished(object sender, EventArgs e)
    {
        if (isConfiguring)
        {
            return;
        }
        ShelfConfiguration = new GitHubShelfConfiguration(gitHubOwnerInput.Text, gitHubRepositoryInput.Text);
        shelf.ResetPage();
        shelf.LoadContent();
    }

    private async void LoadReqBazProjectList()
    {
        shelf.MessageBadge.ShowProcessing();
        ApiResult<Project[]> res = await RequirementsBazaar.GetProjects();
        shelf.MessageBadge.DoneProcessing();
        if (res.Successful)
        {
            projects = res.Value;

            // create the list of project strings for the autocomplete function
            List<string> projectStrings = new List<string>();
            for (int i=0;i<projects.Length;i++)
            {
                projectStrings.Add(projects[i].name);
            }
            reqBazProjectInput.AutocompleteOptions = projectStrings;
        }
        else
        {
            shelf.MessageBadge.ShowMessage(res.ResponseCode);
            projects = null;
        }
        categories = null;
    }

    private async void LoadReqBazCategoryList()
    {
        if (ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR || ((ReqBazShelfConfiguration)ShelfConfiguration).SelectedProject == null)
        {
            // this can only be done if the current configuration is set to requirements bazaar
            return;
        }

        shelf.MessageBadge.ShowProcessing();
        ApiResult<Category[]> res = await RequirementsBazaar.GetCategoriesInProject(((ReqBazShelfConfiguration)ShelfConfiguration).SelectedProject.id);
        shelf.MessageBadge.DoneProcessing();
        if (res.Successful)
        {
            categories = res.Value;
            reqBazCategoryDropdownMenu.Items = new List<Category>(categories);
        }
        else
        {
            shelf.MessageBadge.ShowMessage(res.ResponseCode);
            categories = null;
            reqBazCategoryDropdownMenu.Items = new List<Category>();
        }
    }

    private Project GetReqBazProject(string projectName)
    {
        for (int i = 0; i < projects.Length; i++)
        {
            if (projects[i].name == projectName)
            {
                return projects[i];
            }
        }
        return null;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        // do not set position and eulerAngles since the configuration window should be fixed
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
