using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;

namespace i5.Toolkit.Core.Examples.OpenIDConnectClient
{
    /// <summary>
    /// Bootstrapper for initializing the OpenID Connect service for the github provider
    /// </summary>
    public class GitHubBootstrapper : BaseServiceBootstrapper
    {
        protected override void RegisterServices()
        {
            OpenIDConnectService oidc = new OpenIDConnectService();
            oidc.OidcProvider = new GitHubOidcProvider();

            oidc.RedirectURI = "i5:/";
            ServiceManager.RegisterProvider(oidc, ServiceManager.Provider.GitHub);
        }

        protected override void UnRegisterServices()
        {
            ServiceManager.RemoveProvider<OpenIDConnectService>(ServiceManager.Provider.GitHub);
        }
    }
}