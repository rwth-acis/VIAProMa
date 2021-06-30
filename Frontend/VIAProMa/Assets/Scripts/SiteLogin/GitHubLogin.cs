using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using TMPro;
using i5.VIAProMa.Login;

public class GitHubLogin : ProviderLogin
{
    [Header("Client Credentials")]
    [SerializeField] public ClientDataObject clientDataObject;

    [Header("UI")]
    [SerializeField] public TextMeshPro loginCaption;
    [SerializeField] public TextMeshPro statusCaption;
    [SerializeField] public GameObject statusLed;
    [SerializeField] public Color loggedInColor = new Color(0f, 135f / 255f, 3f / 255f); // green
    [SerializeField] public Color loggedOutColor = new Color(188f / 255f, 2f / 255f, 0f); // red


    public override void Start()
    {
        ServiceManager.GetService<GitHubOidcService>().LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetService<GitHubOidcService>().LogoutCompleted += LoginScript_LogoutCompleted;
        SetLED(false);
    }


    public override void Awake()
    {
        statusLedRenderer = statusLed?.GetComponent<Renderer>();
        oidcProvider = ServiceManager.GetService<GitHubOidcService>().OidcProvider;
    }

    /// <summary>
    /// Adjusts the interface to the logged in status
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public override void LoginScript_LoginCompleted(object sender, System.EventArgs e)
    {
        loggedIn = true;
        loginCaption.text = "Logout:";
        statusCaption.text = "You are logged in!";
        Debug.Log("Successful Login to GitHub");
        SetLED(true);
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
        Debug.Log("Successful Logout from GitHub.");
        SetLED(false);
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
            ServiceManager.GetService<GitHubOidcService>().Scopes = new string[] { "openid", "profile", "email", "rep", "public_repo" };
            ServiceManager.GetService<GitHubOidcService>().OpenLoginPage();
        }
        else
        {
            ServiceManager.GetService<GitHubOidcService>().Logout();
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
            statusLedRenderer.material.SetColor("_EmissiveColor", loggedOutColor);
        }
    }
}
