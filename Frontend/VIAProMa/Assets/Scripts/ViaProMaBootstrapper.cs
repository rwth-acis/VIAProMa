﻿using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Login;

namespace i5.VIAProMa
{

    public class ViaProMaBootstrapper : BaseServiceBootstrapper
    {
        [SerializeField] private ClientDataObject learningLayersClientData;
        [SerializeField] private ClientDataObject gitHubClientData;
        [SerializeField] private ClientDataObject slackClientData;

        protected override void RegisterServices()
        {
            LearningLayersOidcService learningLayersOidcService = new LearningLayersOidcService()
            {
                OidcProvider = new LearningLayersOidcProvider()
            };
            learningLayersOidcService.OidcProvider.ClientData = learningLayersClientData.clientData;
#if !UNITY_EDITOR
            learningLayersOidcService.RedirectURI = "i5:/";
#endif
            ServiceManager.RegisterService(learningLayersOidcService);

            GitHubOidcService githubOidcService = new GitHubOidcService()
            {
                OidcProvider = new GitHubOidcProvider()
            };
            githubOidcService.OidcProvider.ClientData = gitHubClientData.clientData;
#if !UNITY_EDITOR
            githubOidcService.RedirectURI = "i5:/";
#endif
            ServiceManager.RegisterService(githubOidcService);

            SlackOidcService slackOidcService = new SlackOidcService()
            {
                OidcProvider = new SlackOidcProvider()
            };
            slackOidcService.OidcProvider.ClientData = slackClientData.clientData;
#if !UNITY_EDITOR
            slackOidcService.RedirectURI = "i5:/";
#endif
            ServiceManager.RegisterService(slackOidcService);
        }

        protected override void UnRegisterServices()
        {
            ServiceManager.RemoveService<LearningLayersOidcService>();
            ServiceManager.RemoveService<GitHubOidcService>();
            ServiceManager.RemoveService<SlackOidcService>();
        }
    }
}