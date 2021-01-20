using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using TMPro;

public class LoginScript : MonoBehaviour
{
    // expose a field in the inspector for your client credentials
    [SerializeField] private ClientDataObject clientDataObject;

    [Header("UI")]
    [SerializeField] private TextMeshPro loginCaption;
    [SerializeField] private TextMeshPro statusCaption;
    [SerializeField] private GameObject statusLed;
    [SerializeField] private Color loggedInColor = new Color(0f, 135f / 255f, 3f / 255f); // green
    [SerializeField] private Color loggedOutColor = new Color(188f / 255f, 2f / 255f, 0f); // red

    private Renderer statusLedRenderer;

    public IOidcProvider oidcProvider;
    private bool loggedIn = false;

    private void Awake()
    {
        statusLedRenderer = statusLed?.GetComponent<Renderer>();
        oidcProvider = ServiceManager.GetService<OpenIDConnectService>().OidcProvider;
    }

    private void Start()
    {
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetService<OpenIDConnectService>().LogoutCompleted += LoginScript_LogoutCompleted;
        SetLED(false);
    }

    private void LoginScript_LoginCompleted(object sender, System.EventArgs e)
    {
        loggedIn = true;
        loginCaption.text = "Logout:";
        statusCaption.text = "You are logged in!";
        Debug.Log("Successful Login (Learning Layers)");
        SetLED(true);
    }

    private void LoginScript_LogoutCompleted(object sender, System.EventArgs e)
    {
        loggedIn = false;
        loginCaption.text = "Login:";
        statusCaption.text = "You are not logged in yet.";
        Debug.Log("Successful Logout (Learning Layers)");
        SetLED(false);
    }

    // Login process started when the button is clicked
    public void Login()
    {
        if (!loggedIn)
        {
            if (clientDataObject.clientData == null)
                return;
            //first create an instance of the IOidcProvider that should be used and assign the client credentials
            oidcProvider.ClientData = clientDataObject.clientData;

            //assign the instance to the xref:i5.Toolkit.Core.OpenIDConnectClient.IOidcProvider> property of the service
            ServiceManager.GetService<OpenIDConnectService>().OidcProvider = oidcProvider;

            //To start the login process, call the OpenLoginPage() method of the OpenIDConnectService
            ServiceManager.GetService<OpenIDConnectService>().OpenLoginPage();
        }
        else
        {
            ServiceManager.GetService<OpenIDConnectService>().Logout();
        }
    }

    /// <summary>
    /// Sets the emission color of the given LED renderer based on the online status of the login process
    /// </summary>
    /// <param name="ledRenderer">The renderer of the LED to change</param>
    /// <param name="online">True, if the corresponding server is online; otherwise false</param>
    private void SetLED(bool loggedIn)
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
