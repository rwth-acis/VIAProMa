using HoloToolkit.Unity;
using i5.VIAProMa.DataModel.API;
using System;
using System.Collections.Generic;

namespace i5.VIAProMa.IssueAccess
{
    /// <summary>
    /// Manager of the OpenID Connect Client, that allows accessing GitHub and Requirements Bazaar accounts.
    /// </summary>
    public class IssueAccessManager : Singleton<IssueAccessManager>
    {

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void RegisterServices()
        {
            OpenIDConnectService oidc = new OpenIDConnectService();
            oidc.OidcProvider = new LearningLayersOidcProvider();
            // this example shows how the service can be used on an app for multiple platforms
            #if UNITY_WSA
             oidc.RedirectURI = "i5:/";
            #else
            oidc.RedirectURI = "https://www.google.com";
            #endif
            ServiceManager.RegisterService(oidc);
        }

    }
}