using UnityEngine;
using TMPro;
using i5.VIAProMa.DataModel.API;
using System;

public class EditButton : MonoBehaviour
{
    [HideInInspector] public TextMeshPro issueName;
    [HideInInspector] public TextMeshPro issueDescription;
    [HideInInspector] public DataSource source;

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
                if (source == DataSource.REQUIREMENTS_BAZAAR)
                {
                    editMenu.SetText_RequirementBazaar();
                    requirementBazaar_UI.SetActive(true);
                    gitHub_UI.SetActive(false);
                }
                else if (source == DataSource.GITHUB)
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
