using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using i5.VIAProMa.Shelves.IssueShelf;
using Microsoft.MixedReality.Toolkit.UI;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;

public class EditButton : MonoBehaviour
{
    [HideInInspector] public TextMeshPro requirementName;
    [HideInInspector] public TextMeshPro requirementDescription;

    private EditIssueMenu editMenu;
    private bool isOpen = false;
    private ProjectTracker projectTracker;


    public void Start()
    {
        //Get the edit menu from the project tracker
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
        editMenu = projectTracker.editIssueMenu;
    }


    //Close the EditIssue Window
    public void CloseMenu()
    {
        if (editMenu != null)
        {
            editMenu.gameObject.SetActive(false);
        }
        isOpen = false;
    }


    //Open the EditIssue Window
    public void OpenMenu()
    {
         if (editMenu != null)
            {
                editMenu.gameObject.SetActive(true);
                editMenu.requirementName = requirementName;
                editMenu.requirementDescription = requirementDescription;
                editMenu.SetText(); 
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
