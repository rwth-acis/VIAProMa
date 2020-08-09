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
        OpenIDConnectService oidcService = new OpenIDConnectService();
        LearningLayersOIDCProvider provider = new LearningLayersOIDCProvider();
        provider.AuthorizationFlow = AuthorizationFlow.AUTHORIZATION_CODE;
        provider.ClientData = learningLayersClientData.clientData;
        oidcService.OidcProvider = provider;
        oidcService.RedirectURI = "viaproma://";
        ServiceManager.RegisterService(oidcService);
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<OpenIDConnectService>();
    }
}
