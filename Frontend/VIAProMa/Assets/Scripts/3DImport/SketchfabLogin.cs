using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using TMPro;
using i5.VIAProMa.Login;

public class SketchfabLogin : ProviderLogin
{
    [SerializeField] 
    private ClientDataObject sketchfabClientData;

    public override void Start()
    {
        RegisterService();
        ServiceManager.GetService<LearningLayersOidcService>().LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetService<LearningLayersOidcService>().LogoutCompleted += LoginScript_LogoutCompleted;
    }
    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new SketchfabOicdProvider();
        oidc.OidcProvider.ClientData = sketchfabClientData.clientData;
        oidc.RedirectURI = "http://127.0.0.1";
        ServiceManager.RegisterService(oidc);
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
}
