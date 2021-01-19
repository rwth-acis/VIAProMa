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

    [SerializeField] private TextMeshPro loginCaption;

    public IOidcProvider oidcProvider = ServiceManager.GetService<OpenIDConnectService>().OidcProvider;
    private bool loggedIn = false;

    private void Start()
    {
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += LoginScript_LoginCompleted;
        ServiceManager.GetService<OpenIDConnectService>().LogoutCompleted += LoginScript_LogoutCompleted;
    }

    private void LoginScript_LoginCompleted(object sender, System.EventArgs e)
    {
        loggedIn = true;
        loginCaption.text = "Logout";
    }

    private void LoginScript_LogoutCompleted(object sender, System.EventArgs e)
    {
        loggedIn = false;
        loginCaption.text = "Login";
    }

    // Login process started when the button is clicked
    public void Login()
    {
        if (oidcProvider == null)
            Debug.Log("YAY");
        if (!loggedIn)
        {
            if (clientDataObject.clientData == null)
                return;
            //first create an instance of the IOidcProvider that should be used and assign the client credentials
            oidcProvider.ClientData = clientDataObject.clientData;

            //assign the instance to the xref:i5.Toolkit.Core.OpenIDConnectClient.IOidcProvider> property of the service
            //ServiceManager.GetService<OpenIDConnectService>().OidcProvider = oidcProvider;

            //To start the login process, call the OpenLoginPage() method of the OpenIDConnectService
            ServiceManager.GetService<OpenIDConnectService>().OpenLoginPage();
        }
        else
        {
            ServiceManager.GetService<OpenIDConnectService>().Logout();
        }
    }
}
