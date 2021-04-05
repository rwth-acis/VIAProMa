using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using Org.Requirements_Bazaar.API;
using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.DataModel.ReqBaz;
using System.Collections;

public class CreateIssueMenuOpener : MonoBehaviour
{
    [SerializeField] CreateIssueMenu createIssueMenu;
    [SerializeField] GameObject infoText;
    [SerializeField] GameObject backPlate;
    [SerializeField] GameObject notification;

    bool isOpen = false;
    bool isProjectLoaded = false;
    bool categorySelected = false;
    bool isloggedIn = false;

    ReqBazShelfConfiguration reqBazShelfConfiguration;


    public void Start()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        backPlate.SetActive(true);
        infoText.GetComponent<TextMeshPro>().text = "Login or load a project.";
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazProjectChanged += ProjectChanged;
        GameObject.FindObjectOfType<ShelfConfigurationMenu>().ReqBazCategoryChanged += CategoryChanged;
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
    }

    /// <summary>
    /// Sets Project state
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void ProjectChanged(object sender, System.EventArgs e)
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        isProjectLoaded = reqBazShelfConfiguration.IsValidConfiguration;
        Debug.Log(isProjectLoaded);
        if (isloggedIn && isProjectLoaded && categorySelected)
            EnableButton();
        else
            DisableButton();
    }

    /// <summary>
    /// Sets Category state
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public void CategoryChanged(object sender, System.EventArgs e)
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        categorySelected = reqBazShelfConfiguration.SelectedCategory != new Category(-1,"No Category");
        Debug.Log(categorySelected);
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
            EnableButton();
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

    public void CloseMenu()
    {
        createIssueMenu.gameObject.SetActive(false);
        isOpen = false;
    }

    public void OpenMenu()
    {
        reqBazShelfConfiguration = (ReqBazShelfConfiguration)GameObject.FindObjectOfType<ShelfConfigurationMenu>().ShelfConfiguration;
        if (!isloggedIn || !reqBazShelfConfiguration.IsValidConfiguration || reqBazShelfConfiguration.SelectedCategory == new Category(-1, "No Category"))
        {
            notification.SetActive(true);
            StartCoroutine(WaitUntilDeactivate());
        }
        else
        {
            createIssueMenu.gameObject.SetActive(true);
            isOpen = true;
        }
    }

    IEnumerator WaitUntilDeactivate()
    {
        yield return new WaitForSeconds(3f);
        notification.SetActive(false);
    }

    public void DisableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = false;
        backPlate.SetActive(true);
        infoText.GetComponent<TextMeshPro>().text = "Login or load a project.";
    }

    public void EnableButton()
    {
        gameObject.GetComponent<Interactable>().IsEnabled = true;
        backPlate.SetActive(false);
        infoText.GetComponent<TextMeshPro>().text = "Create Issue";
    }

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
