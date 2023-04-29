﻿using UnityEngine;
using TMPro;
using i5.VIAProMa.DataModel.API;
using System;
using i5.VIAProMa.Login;
using System.Collections.Generic;
using i5.VIAProMa.Shelves.IssueShelf;

/// <summary>
/// Contains the functionalities of the edit button for issue cards
/// </summary>
public class EditButton : IssueButton
{
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;

    private EditIssueMenu editMenu;
    private GameObject requirementBazaarUI;
    private GameObject gitHubUI;
    private bool isOpen = false;
    private ProjectTracker projectTracker;

    /// <summary>
    /// Determines whether the issue card is a child of the issue shelf or if it has been dragged out of the shelf
    /// </summary>
    public bool BelongsToShelf
    {
        get
        {
            return this.transform.parent.GetComponentInParent<IssuesLoader>() != null;
        }
    }
    /// <summary>
    /// Sets up the shelf and disables edit buttons for freely placed issue cards
    /// </summary>
    public void Start()
    {
        if (!BelongsToShelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            //Get the edit menu from the project tracker and the UIs
            projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
            editMenu = projectTracker.editIssueMenu;
            requirementBazaarUI = editMenu.requirementBazaar_UI;
            gitHubUI = editMenu.gitHub_UI;
            Setup(new List<DataSource>() { DataSource.GITHUB, DataSource.REQUIREMENTS_BAZAAR });
        }
    }

    /// <summary>
    /// Close the EditIssue Window
    /// </summary>
    public void CloseMenu()
    {
        if (editMenu != null)
        {
            editMenu.gameObject.SetActive(false);
            requirementBazaarUI.SetActive(false);
            gitHubUI.SetActive(false);
        }
        isOpen = false;
    }

    /// <summary>
    /// Open the EditIssue Window for the corresponding Data source
    /// </summary>
    public void OpenMenu()
    {
        if (editMenu != null)
        {
            editMenu.gameObject.SetActive(true);
            editMenu.issueName = issueName;
            editMenu.issueDescription = issueDescription;
            editMenu.issueID = resourceID;
            if (source.Content.Source == DataSource.REQUIREMENTS_BAZAAR)
            {
                editMenu.SetText_RequirementBazaar();
                requirementBazaarUI.SetActive(true);
                gitHubUI.SetActive(false);
            }
            else if (source.Content.Source == DataSource.GITHUB)
            {
                editMenu.SetText_GitHub();
                requirementBazaarUI.SetActive(false);
                gitHubUI.SetActive(true);
            }
        }
        isOpen = true;
    }

    /// <summary>
    /// Either open or close the Edit Issue Window depending on the current state
    /// </summary>
    public void OpenEditIssueMenu()
    {
        if (editMenu != null)
        {
            if (isOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu();
            }
        }
    }
}
