using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using TMPro;

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


    public override void Start()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LoginScript_LogoutCompleted;
        SetLED(false);
    }


    public override void Awake()
    {
        statusLedRenderer = statusLed?.GetComponent<Renderer>();
        oidcProvider = ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).OidcProvider;
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
            if (clientDataObject.clientData == null)
                return;

            //first create an instance of the IOidcProvider that should be used and assign the client credentials
            oidcProvider.ClientData = clientDataObject.clientData;

            //assign the instance to the xref:i5.Toolkit.Core.OpenIDConnectClient.IOidcProvider> property of the service
            ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).OidcProvider = oidcProvider;

            //Define Redirect URI for use on UWP builds
            ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).RedirectURI = "i5:/";

            //To start the login process, call the OpenLoginPage() method of the OpenIDConnectService
            ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).OpenLoginPage();
        }
        else
        {
            ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).Logout();
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
