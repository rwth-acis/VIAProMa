using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.Shelves.IssueShelf;
using System.Collections;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Login;

/// <summary>
/// Manages the active state of the create button of issue shelf
/// </summary>
public class CreateIssueMenuOpener : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] CreateIssueMenu createIssueMenu;
    [SerializeField] GameObject backPlate;
    [SerializeField] GameObject notification;

    bool isOpen = false;

    [Header("ActivationCriteria_RequirementBazaar")]
    bool isProjectLoaded_RequirementBazaar = false;
    bool categorySelected_RequirementBazaar = false;
    bool isloggedIn_RequirementBazaar = false;
    bool isRequirementBazaarOpen = true;

    [Header("ActivationCriteria_GitHub")]
    bool isProjectLoaded_GitHub = false;
    bool isloggedIn_GitHub = false;
    bool isGitHubOpen = false;

    ReqBazShelfConfiguration reqBazShelfConfiguration;
    GitHubShelfConfiguration gitHubShelfConfiguration;
    private ProjectTracker projectTracker;


    public void Start()
    {
        //Disable the Create Button and enable the back plate
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        if(backPlate != null)
        {
            backPlate.SetActive(true);
        }
        //Subscribe to Login and Project events
        ServiceManager.GetService<LearningLayersOidcService>().LoginCompleted += LoginCompleted_RequirementBazaar;
        ServiceManager.GetService<LearningLayersOidcService>().LogoutCompleted += LogoutCompleted_RequirementBazaar;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged_RequirementBazaar;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged_RequirementBazaar;

        ServiceManager.GetService<GitHubOidcService>().LoginCompleted += LoginCompleted_GitHub;
        ServiceManager.GetService<GitHubOidcService>().LogoutCompleted += LogoutCompleted_GitHub;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().GitHubProjectChanged += ProjectChanged_GitHub;

        GameObject.FindObjectOfType<ShelfConfigurationMenu>().SourceChanged += SourceChanged;

        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    /// <summary>
    /// Sets source state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void SourceChanged(object sender, System.EventArgs e)
    {
        isRequirementBazaarOpen = GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration.SelectedSource == DataSource.REQUIREMENTS_BAZAAR;
        isGitHubOpen = GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration.SelectedSource == DataSource.GITHUB;

        if (isRequirementBazaarOpen)
        {
            RequirementBazaarCheck();
        }
        if (isGitHubOpen)
        {
            GitHubCheck();
        }
    }

    /// <summary>
    /// Sets RequirementBazaar Project state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged_RequirementBazaar(object sender, System.EventArgs e)
    {
        if (isRequirementBazaarOpen)
        {
            reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
            isProjectLoaded_RequirementBazaar = reqBazShelfConfiguration.IsValidConfiguration;
            RequirementBazaarCheck();
        }
    }

    /// <summary>
    /// Sets Category state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void CategoryChanged_RequirementBazaar(object sender, System.EventArgs e)
    {
        if (isRequirementBazaarOpen)
        {
            reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
            categorySelected_RequirementBazaar = reqBazShelfConfiguration.SelectedCategory != null;
            RequirementBazaarCheck();
        }
    }

    /// <summary>
    /// Adjusts the status of the button according to being logged in to LearningLayers
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void LoginCompleted_RequirementBazaar(object sender, System.EventArgs e)
    {
        isloggedIn_RequirementBazaar = true;

        if (isRequirementBazaarOpen)
        {
            RequirementBazaarCheck();
        }
    }

    /// <summary>
    /// Adjusts the status of the button according to being logged out from LearningLayers
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted_RequirementBazaar(object sender, System.EventArgs e)
    {
        isloggedIn_RequirementBazaar = false;

        if (isRequirementBazaarOpen)
        {
            DisableButton();
        }
    }

    /// <summary>
    /// Sets GitHub Project state and enables the button accordingly
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged_GitHub(object sender, System.EventArgs e)
    {
        if (isGitHubOpen)
        {
            gitHubShelfConfiguration = (GitHubShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
            isProjectLoaded_GitHub = gitHubShelfConfiguration.IsValidConfiguration;
            GitHubCheck();
        }
    }


        /// <summary>
        /// Adjusts the status of the button according to being logged in to GitHub
        /// </summary>
        /// <param name="sender">Sender of event</param>. 
        /// <param name="e">Event arguments</param>
        public void LoginCompleted_GitHub(object sender, System.EventArgs e)
    {
        isloggedIn_GitHub = true;

        if (isGitHubOpen)
        {
            GitHubCheck();
        }
    }

    /// <summary>
    /// Adjusts the status of the button according to being logged out from GitHub
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public void LogoutCompleted_GitHub(object sender, System.EventArgs e)
    {
        isloggedIn_GitHub = false;

        if (isGitHubOpen)
        {
            DisableButton();
        }
    }

    /// <summary>
    /// Close the CreateIssue Window
    /// </summary>
    public void CloseMenu()
    {
        if(createIssueMenu != null)
        {
            createIssueMenu.gameObject.SetActive(false);
        }
        isOpen = false;
    }

    /// <summary>
    /// Open the CreateIssue Window if the configuration of project and category is valid, otherwise enable the notification
    /// </summary>
    public void OpenMenu()
    {
        if (createIssueMenu != null)
        {
            createIssueMenu.gameObject.SetActive(true);
        }
        isOpen = true;
    }

    /// <summary>
    /// Set the notification to enabled for 3 seconds
    /// </summary>
    public void EnableNotification()
    {
        if (notification != null)
        {
            notification.SetActive(true);
        }
        StartCoroutine(WaitUntilDeactivate());
    }

    /// <summary>
    /// Enables the notification after 3 seconds
    /// </summary>
    IEnumerator WaitUntilDeactivate()
    {
        yield return new WaitForSeconds(3f);
        if (notification != null)
        {
            notification.SetActive(false);
        }
    }

    /// <summary>
    /// Disable the Create Issue Button
    /// </summary>
    public void DisableButton()
    {
        if (gameObject != null)
        {
            if (gameObject.GetComponent<Interactable>() != null)
            {
                gameObject.GetComponent<Interactable>().IsEnabled = false;
            }
            if (backPlate != null)
            {
                backPlate.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Disables the button 
    /// </summary>
    public void OnDestroy()
    {
        DisableButton();
    }

    /// <summary>
    /// Enable the Create Issue Button
    /// </summary>
    public void EnableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = true;
        if (backPlate != null)
        {
            backPlate.SetActive(false);
        }
    }

    /// <summary>
    /// Either open or close the Create Issue Window depending on the current state
    /// </summary>
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

    /// <summary>
    /// Checks if the Button should be Enabled if a RequirementBazaar Configuration is enabled
    /// </summary>
    private void RequirementBazaarCheck()
    {
        if (isloggedIn_RequirementBazaar && isProjectLoaded_RequirementBazaar)
            EnableButton();
        else
            DisableButton();
    }

    /// <summary>
    /// Checks if the Button should be Enabled if a GitHub Configuration is enabled
    /// </summary>
    private void GitHubCheck()
    {
        if (isloggedIn_GitHub && isProjectLoaded_GitHub)
            EnableButton();
        else
            DisableButton();
    }
}
