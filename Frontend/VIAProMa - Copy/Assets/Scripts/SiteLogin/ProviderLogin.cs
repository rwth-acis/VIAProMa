using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.DataModel.ReqBaz;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.Shelves.IssueShelf;
using TMPro;

/// <summary>
/// Implementation of the Login
/// </summary>
public abstract class ProviderLogin : MonoBehaviour
{
    /// LED renderer
    protected Renderer statusLedRenderer;

    /// The provider used for the login
    protected IOidcProvider oidcProvider;

    /// The current login status
    protected bool loggedIn = false;


    public abstract void Awake();

    public abstract void Start();

    /// <summary>
    /// Adjusts the interface to the logged in status
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public abstract void LoginScript_LoginCompleted(object sender, System.EventArgs e);

    /// <summary>
    /// Adjusts the interface to the logged out status
    /// </summary>
    /// <param name="sender">Sender of event</param>
    /// <param name="e">Event arguments</param>
    public abstract void LoginScript_LogoutCompleted(object sender, System.EventArgs e);

    /// <summary>
    /// Redirects the user to the login page of the provider, if already logged in, the user is logged out
    /// </summary>
    public abstract void Login();


    /// <summary>
    /// Sets the emission color of the given LED renderer based on the online status of the login process
    /// </summary>
    /// <param name="ledRenderer">The renderer of the LED to change</param>
    /// <param name="online">True, if the corresponding server is online; otherwise false</param>
    public abstract void SetLED(bool loggedIn);
}
