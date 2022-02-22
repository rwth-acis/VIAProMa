using UnityEngine;
using TMPro;
using i5.VIAProMa.DataModel.API;
using System;
using i5.VIAProMa.Login;
using System.Collections.Generic;

public class EditButton : IssueButton
{
    [SerializeField] private TextMeshPro issueName;
    [SerializeField] private TextMeshPro issueDescription;

    private EditIssueMenu editMenu;
    private GameObject requirementBazaar_UI;
    private GameObject gitHub_UI;
    private bool isOpen = false;
    private ProjectTracker projectTracker;

    public void Start()
    {
        //Get the edit menu from the project tracker and the UIs
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
        editMenu = projectTracker.editIssueMenu;
        requirementBazaar_UI = editMenu.requirementBazaar_UI;
        gitHub_UI = editMenu.gitHub_UI;
        Setup(new List<DataSource>() { DataSource.GITHUB, DataSource.REQUIREMENTS_BAZAAR });
    }


    //Close the EditIssue Window
    public void CloseMenu()
    {
        if (editMenu != null)
        {
            editMenu.gameObject.SetActive(false);
            requirementBazaar_UI.SetActive(false);
            gitHub_UI.SetActive(false);
        }
        isOpen = false;
    }


    //Open the EditIssue Window
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
                    requirementBazaar_UI.SetActive(true);
                    gitHub_UI.SetActive(false);
                }
                else if (source.Content.Source == DataSource.GITHUB)
                {
                    editMenu.SetText_GitHub();
                    requirementBazaar_UI.SetActive(false);
                    gitHub_UI.SetActive(true);
            }
            }
            isOpen = true;
    }
   

    //Either open or close the Edit Issue Window depending on the current state
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
