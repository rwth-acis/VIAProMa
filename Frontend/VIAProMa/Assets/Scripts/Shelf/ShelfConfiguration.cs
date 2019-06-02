using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfConfiguration : MonoBehaviour
{
    [SerializeField] private StringDropdownMenu sourceSelection;
    [SerializeField] private InputField projectInput;
    [SerializeField] private CategoryDropdownMenu categoryDropdownMenu;

    private ILoadShelf shelf;

    Project[] projects;

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
        }
    }

    private void ProjectSelected(object sender, EventArgs e)
    {
    }

    private void CategorySelected(object sender, EventArgs e)
    {
    }
}
