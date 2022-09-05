using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;
using i5.VIAProMa.Shelves.IssueShelf;
using System.Collections;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.Login;

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
        if (createIssueMenu != null)
        {
            createIssueMenu.gameObject.SetActive(true);
        }
        isOpen = true;
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

    public void OnDestroy()
    {
        DisableButton();
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

    //Checks if the Button should be Enabled if a RequirementBazaar Configuration is enabled
    private void RequirementBazaarCheck()
    {
        if (isloggedIn_RequirementBazaar && isProjectLoaded_RequirementBazaar)
            EnableButton();
        else
            DisableButton();
    }

    //Checks if the Button should be Enabled if a GitHub Configuration is enabled
    private void GitHubCheck()
    {
        if (isloggedIn_GitHub && isProjectLoaded_GitHub)
            EnableButton();
        else
            DisableButton();
    }
}
