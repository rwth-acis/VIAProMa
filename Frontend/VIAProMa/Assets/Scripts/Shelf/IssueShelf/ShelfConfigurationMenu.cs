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

    public event EventHandler SourceChanged;
    public event EventHandler ReqBazProjectChanged;
    public event EventHandler ReqBazCategoryChanged;
    public event EventHandler GitHubOwnerChanged;
    public event EventHandler GitHubProjectChanged;
    public event EventHandler WindowOpened;
    public event EventHandler WindowClosed;

    private bool isConfiguring = true;

    public IShelfConfiguration ShelfConfiguration { get; private set; }

    public bool WindowEnabled
    { // not needed for configuration window => does not have an effect
        get; set;
    }

    public string GitHubOwner { get => gitHubOwnerInput.Text; }
    public string GitHubRepository { get => gitHubRepositoryInput.Text; }

    public bool WindowOpen { get; private set; }

    public bool ExternalSetInProgress { get; set; }

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
    }

    private void Start()
    {
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

        SetDataSource(DataSource.REQUIREMENTS_BAZAAR); // first entry of dropdown box is Requirements Bazaar, so set this as the default source

        // initialize the text fields
        reqBazProjectInput.Text = "";
        gitHubOwnerInput.Text = "";
        gitHubRepositoryInput.Text = "";

        isConfiguring = false;
    }

    private void SourceSelected(object sender, EventArgs e)
    {
        DataSource selectedSource = (DataSource)sourceSelection.SelectedItemIndex;
        SetDataSource(selectedSource);
        SourceChanged?.Invoke(this, EventArgs.Empty); // important: invoke it only if the user changes the source
    }

    public void SetDataSource(DataSource selectedSource)
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
                        if (reqBazCategoryDropdownMenu.SelectedItem.id == -1)
                        {
                            ShelfConfiguration = new ReqBazShelfConfiguration(GetReqBazProject(reqBazProjectInput.Text));
                        }
                        else
                        {
                            ShelfConfiguration = new ReqBazShelfConfiguration(GetReqBazProject(reqBazProjectInput.Text), reqBazCategoryDropdownMenu.SelectedItem);
                        }
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
        sourceSelection.SelectedItemIndex = (int)selectedSource;
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
        Debug.Log("Project input finished");
        bool successful = SetReqBazProject();
        if (successful)
        {
            ReqBazProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool SetReqBazProject()
    {
        if (isConfiguring)
        {
            return false;
        }
        if (ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR || string.IsNullOrEmpty(reqBazProjectInput.Text))
        {
            return false;
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
            return false;
        }
        Project selectedProject = GetReqBazProject(reqBazProjectInput.Text);
        ShelfConfiguration = new ReqBazShelfConfiguration(selectedProject);
        if (selectedProject == null) // project was not found
        {
            Debug.LogWarning("Project not found");
            return false;
        }
        else // fetch categories
        {
            LoadReqBazCategoryList();
            shelf.ResetPage();
            shelf.LoadContent();
            return true;
        }
    }

    private void ReqBazCategorySelected(object sender, EventArgs e)
    {
        SetReqBazCategory();
        ReqBazCategoryChanged?.Invoke(this, EventArgs.Empty);
    }

    private bool SetReqBazCategory()
    {
        if (isConfiguring || ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR)
        {
            return false;
        }

        if (categories == null)
        {
            LoadReqBazCategoryList();
        }
        // if still no categories found => show error message
        if (categories == null)
        {
            shelf.MessageBadge.ShowMessage(0);
            return false;
        }

        Category selectedCategory = categories[reqBazCategoryDropdownMenu.SelectedItemIndex];
        if (selectedCategory.id == -1) // pseudo category for "no category" selection
        {
            ((ReqBazShelfConfiguration)ShelfConfiguration).SelectedCategory = null;
        }
        else
        {
            ((ReqBazShelfConfiguration)ShelfConfiguration).SelectedCategory = selectedCategory;
        }
        shelf.ResetPage();
        shelf.LoadContent();
        return true;
    }

    private void GitHubOwnerInputFinished(object sender, EventArgs e)
    {
        if (!ExternalSetInProgress)
        {
            SetGitHubOwner();
            GitHubOwnerChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetGitHubOwner()
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
        if (!ExternalSetInProgress)
        {
            SetGitHubProject();
            GitHubProjectChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetGitHubProject()
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
            for (int i = 0; i < projects.Length; i++)
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
            List<Category> resCategories = new List<Category>(res.Value);
            resCategories.Insert(0, new Category(-1, "No Category"));
            categories = resCategories.ToArray();
            reqBazCategoryDropdownMenu.Items = resCategories;
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

    public void SetReqBazProject(int projectId)
    {
        for (int i = 0; i < projects.Length; i++)
        {
            if (projects[i].id == projectId)
            {
                reqBazProjectInput.Text = projects[i].name;
                SetReqBazProject();
                break;
            }
        }
    }

    public void SetReqBazCategory(int categoryId)
    {
        for (int i = 0; i < reqBazCategoryDropdownMenu.Items.Count; i++)
        {
            if (reqBazCategoryDropdownMenu.Items[i].id == categoryId)
            {
                reqBazCategoryDropdownMenu.SelectedItemIndex = i;
                SetReqBazCategory();
                break;
            }
        }
        Debug.LogError("Req Baz Category could not be bound", gameObject);
    }

    public void SetGitHubOwner(string owner)
    {
        ExternalSetInProgress = true;
        gitHubOwnerInput.Text = owner;
        SetGitHubOwner();
        ExternalSetInProgress = false;
    }

    public void SetGitHubProject(string project)
    {
        ExternalSetInProgress = true;
        gitHubRepositoryInput.Text = project;
        SetGitHubProject();
        ExternalSetInProgress = false;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
        if (!ExternalSetInProgress)
        {
            WindowOpened?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        // do not set position and eulerAngles since the configuration window should be fixed
    }

    public void Close()
    {
        WindowOpen = false;
        gameObject.SetActive(false);
        if (!ExternalSetInProgress)
        {
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }
    }
}
