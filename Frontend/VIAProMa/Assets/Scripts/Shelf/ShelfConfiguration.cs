using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfConfiguration : MonoBehaviour
{
    [SerializeField] private RequirementsLoader shelf;

    [SerializeField] private StringDropdownMenu sourceSelection;
    [SerializeField] private InputField projectInput;
    [SerializeField] private CategoryDropdownMenu categoryDropdownMenu;

    private Project[] projects;
    private Category[] categories;

    public Project SelectedProject { get; private set; }

    public Category SelectedCategory { get; private set; }

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

        sourceSelection.ItemSelected += SourceSelected;
        projectInput.TextChanged += ProjectInputFinished;
        categoryDropdownMenu.ItemSelected += CategorySelected;

        LoadProjectList();
    }

    private void SourceSelected(object sender, EventArgs e)
    {
        LoadProjectList();
    }

    private void ProjectInputFinished(object sender, EventArgs e)
    {
        SelectedProject = GetProject(projectInput.Text);
        if (SelectedProject == null) // project was not found
        {
            Debug.LogWarning("Project not found");
        }
        else // fetch categories
        {
            LoadCategoryList();
        }
    }

    private void CategorySelected(object sender, EventArgs e)
    {
        SelectedCategory = categories[categoryDropdownMenu.SelectedItemIndex];
    }

    private async void LoadProjectList()
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

        for(int i=0;i<projects.Length;i++)
        {
            Debug.Log(projects[i].name);
        }
    }

    private async void LoadCategoryList()
    {
        shelf.MessageBadge.ShowProcessing();
        ApiResult<Category[]> res = await RequirementsBazaar.GetCategoriesInProject(SelectedProject.id);
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
}
