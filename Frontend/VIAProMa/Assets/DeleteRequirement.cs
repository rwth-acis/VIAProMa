using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;

public class DeleteRequirement : MonoBehaviour
{
    private bool isAuthenticated_ReqBaz;
    private bool isAuthenticated_GitHub;
    private bool isInDeleteMode;
    [SerializeField] GameObject DeleteButton;

    public void Start()
    {
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted_ReqBaz;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted_ReqBaz;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LoginCompleted += LoginCompleted_GitHub;
        ServiceManager.GetProvider<OpenIDConnectService>(ProviderTypes.LearningLayers).LogoutCompleted += LogoutCompleted_GitHub;
        isAuthenticated_ReqBaz = false;
        isAuthenticated_GitHub = false;
    }

    public void LoginCompleted_ReqBaz(object sender, System.EventArgs e)
    {
        isAuthenticated_ReqBaz = true;
    }

    public void LogoutCompleted_ReqBaz(object sender, System.EventArgs e)
    {
        isAuthenticated_ReqBaz = false;
    }

    public void LoginCompleted_GitHub(object sender, System.EventArgs e)
    {
        isAuthenticated_GitHub = true;
    }

    public void LogoutCompleted_GitHub(object sender, System.EventArgs e)
    {
        isAuthenticated_GitHub = false;
    }
}