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

    [Header("UI Elements")]
    [SerializeField] GameObject backPlate;
    private EditIssueMenu editMenu;

    [Header("ActivationCriteria")]
    bool isOpen = false;
    bool isProjectLoaded = false;
    bool categorySelected = false;
    bool isloggedIn = false;

    ReqBazShelfConfiguration reqBazShelfConfiguration;
    private ProjectTracker projectTracker;


    public void Start()
    {
        //Disable the Edit Button and enable the back plate
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        if (backPlate != null)
        {
            backPlate.SetActive(true);
        }
        //Subscribe to Login and Project events
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;

        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
        editMenu = GameObject.FindObjectOfType<EditIssueMenu>();
    }


    /// <summary>
    /// Sets Project state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged(object sender, System.EventArgs e)
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        isProjectLoaded = reqBazShelfConfiguration.IsValidConfiguration;

        if (isloggedIn && isProjectLoaded && categorySelected)
            EnableButton();
        else
            DisableButton();
    }

    /// <summary>
    /// Sets Category state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void CategoryChanged(object sender, System.EventArgs e)
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        categorySelected = reqBazShelfConfiguration.SelectedCategory != null;

        if (isloggedIn && isProjectLoaded && categorySelected)
            EnableButton();
        else
            DisableButton();
    }

    /// <summary>
    /// Adjusts the status of the button according to being logged in
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void LoginCompleted(object sender, System.EventArgs e)
    {
        isloggedIn = true;
        if (isloggedIn && isProjectLoaded && categorySelected)
        {
            EnableButton();
        }
    }

    /// <summary>
    /// Adjusts the status of the button according to being logged out
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted(object sender, System.EventArgs e)
    {
        isloggedIn = false;
        DisableButton();
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

    //Open the EditIssue Window if the configuration of project and category is valid
    public void OpenMenu()
    {
        if (!isloggedIn || projectTracker.currentProjectID == 0 || projectTracker.currentCategory == null)
        {
        }
        else
        {
            if (editMenu != null)
            {
                editMenu.gameObject.SetActive(true);
                editMenu.requirementName = requirementName;
                editMenu.requirementDescription = requirementDescription;
            }
            isOpen = true;
        }
    }


    //Disable the Edit Issue Button
    public void DisableButton()
    {
        if (gameObject != null || gameObject.GetComponent<Interactable>() != null)
        {
            gameObject.GetComponent<Interactable>().IsEnabled = false;
        }
        if (backPlate != null)
        {
            backPlate.SetActive(true);
        }
    }

    //Enable the Edit Issue Button
    public void EnableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = true;
        if (backPlate != null)
        {
            backPlate.SetActive(false);
        }
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
