using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfConfiguration : MonoBehaviour
{
    private RequirementsLoader shelf;

    [SerializeField] private StringDropdownMenu sourceSelection;
    [SerializeField] private InputField projectInput;
    [SerializeField] private CategoryDropdownMenu categoryDropdownMenu;

    private Project[] projects;
    private Category[] categories;

    public Project SelectedProject { get; private set; }

    public Category SelectedCategory { get; private set; }

    private void Awake()
    {
        sourceSelection.ItemSelected += SourceSelected;
        projectInput.TextChanged += ProjectSelected;
        categoryDropdownMenu.ItemSelected += CategorySelected;
    }

    private async void SourceSelected(object sender, EventArgs e)
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
        SelectedProject = null;
        SelectedCategory = null;
    }

    private async void ProjectSelected(object sender, EventArgs e)
    {
        int projectId = GetProjectId(projectInput.Text);
        if (projectId < 0) // project was not found
        {
            Debug.LogWarning("Project not found");
        }
        else // fetch categories
        {
            shelf.MessageBadge.ShowProcessing();
            ApiResult<Category[]> res = await RequirementsBazaar.GetCategoriesInProject(id);
            shelf.MessageBadge.DoneProcessing();
            if (res.Successful)
            {
                categories = res.Value;
            }
            else
            {
                shelf.MessageBadge.ShowMessage(res.ResponseCode);
                categories = null;
            }
        }
    }

    private void CategorySelected(object sender, EventArgs e)
    {
    }

    private int GetProjectId(string projectName)
    {
        for(int i=0;i<projects.Length;i++)
        {
            if (projects[i].name == projectName)
            {
                return projects[i].id;
            }
        }
        return -1;
    }
}
