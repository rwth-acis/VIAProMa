using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.Shelves.IssueShelf;
using System.Collections;

public class CreateIssueMenuOpener : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] CreateIssueMenu createIssueMenu;
    [SerializeField] GameObject backPlate;
    [SerializeField] GameObject notification;

    [Header("ActivationCriteria")]
    bool isOpen = false;
    bool isProjectLoaded = false;
    bool categorySelected = false;
    bool isloggedIn = false;

    ReqBazShelfConfiguration reqBazShelfConfiguration;


    public void Start()
    {
        //Disable the Create Button and enable the back plate
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        if(backPlate != null)
        {
            backPlate.SetActive(true);
        }
    }


    //Subscribe to Login and Project events
    public void OnEnable()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;
    }

    //Unsubscribe to Login and Project events
    public void OnDisable()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted -= LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted -= LogoutCompleted;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged -= ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged -= CategoryChanged;
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


    //Close the CreateIssue Window
    public void CloseMenu()
    {
        if(createIssueMenu != null)
        {
            createIssueMenu.gameObject.SetActive(false);
        }
        isOpen = false;
    }

    //Open the CreateIssue Window if the configuration of project and category is valid, otherwise enable the notification
    public void OpenMenu()
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        if (!isloggedIn || !reqBazShelfConfiguration.IsValidConfiguration || reqBazShelfConfiguration.SelectedCategory == null)
        {
            EnableNotification();
        }
        else
        {
            if (createIssueMenu != null)
            {
                createIssueMenu.gameObject.SetActive(true);
            }
            isOpen = true;
        }
    }

    //Set the notification to enabled for 3 seconds
    public void EnableNotification()
    {
        if (notification != null)
        {
            notification.SetActive(true);
        }
        StartCoroutine(WaitUntilDeactivate());
    }

    IEnumerator WaitUntilDeactivate()
    {
        yield return new WaitForSeconds(3f);
        if (notification != null)
        {
            notification.SetActive(false);
        }
    }

    //Disable the Create Issue Button
    public void DisableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        if (backPlate != null)
        {
            backPlate.SetActive(true);
        }
    }

    //Enable the Create Issue Button
    public void EnableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = true;
        if (backPlate != null)
        {
            backPlate.SetActive(false);
        }
    }

    //Either open or close the Create Issue Window depending on the current state
    public void OpenCreateIssueMenu()
    {
        if(createIssueMenu != null)
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
