using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfConfigurationMenu : MonoBehaviour, IWindow
{
    [SerializeField] private IssuesLoader shelf;

    [SerializeField] private StringDropdownMenu sourceSelection;
    [SerializeField] private InputField projectInput;
    [SerializeField] private CategoryDropdownMenu categoryDropdownMenu;

    private Project[] projects;
    private Category[] categories;

    public event EventHandler WindowClosed;

    public IShelfConfiguration ShelfConfiguration { get; private set; }

    public bool WindowEnabled { // not needed for configuration window => does not have an effect
        get;set;
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
        if (projectInput == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(projectInput));
        }
        if (categoryDropdownMenu == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(categoryDropdownMenu));
        }

        // populate the source dropdown menu with the available data sources
        List<StringData> sources = new List<StringData>();
        foreach(DataSource source in Enum.GetValues(typeof(DataSource)))
        {
            sources.Add(new StringData(source.GetDescription()));
        }
        sourceSelection.Items = sources;

        sourceSelection.ItemSelected += SourceSelected;
        projectInput.TextChanged += ProjectInputFinished;
        categoryDropdownMenu.ItemSelected += CategorySelected;

        ShelfConfiguration = new ReqBazShelfConfiguration();
        LoadReqBazProjectList();
    }

    private void SourceSelected(object sender, EventArgs e)
    {
        LoadReqBazProjectList();
        DataSource selectedSource = (DataSource)sourceSelection.SelectedItemIndex;
        switch(selectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                ShelfConfiguration = new ReqBazShelfConfiguration();
                break;
            case DataSource.GITHUB:
                break;
        }
        shelf.LoadContent();
    }

    private void ProjectInputFinished(object sender, EventArgs e)
    {
        if (ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            Project selectedProject = GetProject(projectInput.Text);
            ShelfConfiguration = new ReqBazShelfConfiguration(selectedProject);
            if (selectedProject == null) // project was not found
            {
                Debug.LogWarning("Project not found");
            }
            else // fetch categories
            {
                LoadReqBazCategoryList();
            }
        }
        else if (ShelfConfiguration.SelectedSource == DataSource.GITHUB)
        {
            throw new NotImplementedException();
        }
        shelf.ResetPage();
        shelf.LoadContent();
    }

    private void CategorySelected(object sender, EventArgs e)
    {
        Category selectedCategory = categories[categoryDropdownMenu.SelectedItemIndex];
        if (ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR)
        {
            ((ReqBazShelfConfiguration)ShelfConfiguration).SelectedCategory = selectedCategory;
        }
        else if (ShelfConfiguration.SelectedSource == DataSource.GITHUB)
        {
            throw new NotImplementedException();
        }
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
        if (ShelfConfiguration.SelectedSource != DataSource.REQUIREMENTS_BAZAAR)
        {
            return;
        }

        shelf.MessageBadge.ShowProcessing();
        ApiResult<Category[]> res = await RequirementsBazaar.GetCategoriesInProject(((ReqBazShelfConfiguration)ShelfConfiguration).SelectedProject.id);
        shelf.MessageBadge.DoneProcessing();
        if (res.Successful)
        {
            categories = res.Value;
            categoryDropdownMenu.Items = new List<Category>(categories);
        }
        else
        {
            shelf.MessageBadge.ShowMessage(res.ResponseCode);
            categories = null;
            categoryDropdownMenu.Items = new List<Category>();
        }
    }

    private Project GetProject(string projectName)
    {
        for(int i=0;i<projects.Length;i++)
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

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
