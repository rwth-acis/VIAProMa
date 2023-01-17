using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using TMPro;
using i5.VIAProMa.Login;
using i5.VIAProMa.Shelves.IssueShelf;
using System.Threading.Tasks;
using i5.VIAProMa.UI.MultiListView.Core;

public class LearningLayersLogin : ProviderLogin
{
    [Header("Client Credentials")]
    [SerializeField] public ClientDataObject clientDataObject;

    [Header("UI")]
    [SerializeField] public TextMeshPro loginCaption;
    [SerializeField] public TextMeshPro statusCaption;
    [SerializeField] public GameObject statusLed;
    [SerializeField] public Color loggedInColor = new Color(0f, 135f / 255f, 3f / 255f); // green
    [SerializeField] public Color loggedOutColor = new Color(188f / 255f, 2f / 255f, 0f); // red

    private ShelfConfigurationMenu scm;
    private IssuesLoader issueLoader;

    public override void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();

        ServiceManager.GetService<LearningLayersOidcService>().LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetService<LearningLayersOidcService>().LogoutCompleted += LoginScript_LogoutCompleted;
        SetLED(false);
        scm = this.transform.parent.parent.GetComponent<ShelfConfigurationMenu>();
    }


    public override void Awake()
    {
        statusLedRenderer = statusLed?.GetComponent<Renderer>();
        oidcProvider = ServiceManager.GetService<LearningLayersOidcService>().OidcProvider;
    }

    /// <summary>
    /// Adjusts the interface to the logged in status
    /// </summary>
    /// <param name="sender">Sender of event</param>. 
    /// <param name="e">Event arguments</param>
    public override void LoginScript_LoginCompleted(object sender, System.EventArgs e)
    {
        loggedIn = true;
        loginCaption.text = "Logout:";
        statusCaption.text = "You are logged in!";
        Debug.Log("Successful Login to Learning Layers");
        SetLED(true);
        scm.LoadReqBazProjectList();
    }

    /// <summary>
    /// Adjusts the interface to the logged out status
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public override void LoginScript_LogoutCompleted(object sender, System.EventArgs e)
    {
        loggedIn = false;
        loginCaption.text = "Login:";
        statusCaption.text = "You are not logged in yet.";
        Debug.Log("Successful Logout from Learning Layers.");
        SetLED(false);
        scm.LoadReqBazProjectList();
        this.transform.parent.parent.parent.GetComponent<IssuesMultiListView>().Clear();
        issueLoader.LoadContent();
    }

    /// <summary>
    /// Redirects the user to the learning layers login page of the provider, if already logged in, the user is logged out
    /// </summary>
    public override void Login()
    {
        if (!loggedIn)
        {
            if (clientDataObject == null)
            {
                return;
            }

            //To start the login process, call the OpenLoginPage() method of the OpenIDConnectService
            ServiceManager.GetService<LearningLayersOidcService>().OpenLoginPage();
        }
        else
        {
            ServiceManager.GetService<LearningLayersOidcService>().Logout();
        }
    }

    /// <summary>
    /// Sets the emission color of the given LED renderer based on the online status of the login process
    /// </summary>
    /// <param name="ledRenderer">The renderer of the LED to change</param>
    /// <param name="online">True, if the corresponding server is online; otherwise false</param>
    public override void SetLED(bool loggedIn)
    {
        if (loggedIn)
        {
            statusLedRenderer.material.SetColor("_EmissiveColor", loggedInColor);
        }
        else
        {
            if(statusLedRenderer !=  null)
                statusLedRenderer.material.SetColor("_EmissiveColor", loggedOutColor);
        }
    }
}
