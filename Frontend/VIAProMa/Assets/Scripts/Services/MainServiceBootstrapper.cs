using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainServiceBootstrapper : BaseServiceBootstrapper
{
    [SerializeField] private ClientDataObject learningLayersClientData;

    protected override void RegisterServices()
    {
        if (learningLayersClientData == null)
        {
            Debug.LogWarning("No OpenID Connect Client data provided. Login functionality deactivated.\n" +
                "To enable the login functionality, register a Learning Layers client " +
                "and create a client credentials file (right click in the assets browser, " +
                "select Create > i5 Toolkit > OpenID Connect Client Data) and reference it in the bootstrapper",
                this);
        }
        else
        {
            OpenIDConnectService oidcService = new OpenIDConnectService();
            LearningLayersOIDCProvider provider = new LearningLayersOIDCProvider();
            provider.AuthorizationFlow = AuthorizationFlow.AUTHORIZATION_CODE;
            provider.ClientData = learningLayersClientData.clientData;
            oidcService.OidcProvider = provider;
#if UNITY_WSA
            oidcService.RedirectURI = "viaproma:/";
#endif
            ServiceManager.RegisterService(oidcService);
        }
    }

    protected override void UnRegisterServices()
    {
        if (ServiceManager.ServiceExists<OpenIDConnectService>())
        {
            ServiceManager.RemoveService<OpenIDConnectService>();
        }
    }
}
