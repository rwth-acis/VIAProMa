﻿using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IssuesLoader : Shelf, ILoadShelf
{
    [SerializeField] private ShelfConfiguration configuration;
    [SerializeField] private IssuesMultiListView issuesMultiListView;
    [SerializeField] private InputField searchField;

    public MessageBadge MessageBadge { get => messageBadge; }

    private Issue[] issues;
    private Issue[] nextIssues;

    private int page;

    protected override void Awake()
    {
        base.Awake();
        if (configuration == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configuration));
        }
        if(issuesMultiListView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issuesMultiListView));
        }
        if (searchField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchField));
        }
    }

    public void ResetPage()
    {
        page = 0;
    }

    public void LoadContent()
    {
        switch (configuration.SelectedSource)
        {
            case DataSource.REQUIREMENTS_BAZAAR:
                LoadRequirements();
                break;
            case DataSource.GITHUB:
                break;
        }
    }

    private async void LoadRequirements()
    {
        messageBadge.ShowLoadMessage();
        ApiResult<Issue[]> apiResult = null;
        // load requirements from the correct project or category
        if (configuration.SelectedCategory != null) // project and category were selected
        {
            apiResult = await RequirementsBazaar.GetRequirementsInCategory(configuration.SelectedCategory.id, page, issuesMultiListView.numberOfItemsPerListView * issuesMultiListView.NumberOfListViews, searchField.Text);
        }
        else if (configuration.SelectedProject != null) // just a project was selected
        {
            apiResult = await RequirementsBazaar.GetRequirementsInProject(configuration.SelectedProject.id, page, issuesMultiListView.numberOfItemsPerListView * issuesMultiListView.NumberOfListViews, searchField.Text);
        }
        else
        {
            issuesMultiListView.Clear();
        }
        messageBadge.DoneProcessing();
        if (apiResult != null) // web request was made
        {
            if (apiResult.HasError)
            {
                messageBadge.ShowMessage(apiResult.ResponseCode);
            }
            else
            {
                List<Issue> items = new List<Issue>(apiResult.Value);
                issuesMultiListView.Items = items;
            }
        }
    }

    public void ScrollUp()
    {
        page--;
        LoadContent();
    }

    public void ScrollDown()
    {
        page++;
        LoadContent();
    }
}
