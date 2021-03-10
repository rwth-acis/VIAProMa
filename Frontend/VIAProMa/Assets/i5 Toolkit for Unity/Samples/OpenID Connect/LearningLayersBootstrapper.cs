using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;

namespace i5.Toolkit.Core.Examples.OpenIDConnectClient
{
    /// <summary>
    /// Bootstrapper for initializing the OpenID Connect service for the learning layers provider
    /// </summary>
    public class LearningLayersBootstrapper : BaseServiceBootstrapper
    {
        protected override void RegisterServices()
        {
            OpenIDConnectService oidc = new OpenIDConnectService();
            oidc.OidcProvider = new LearningLayersOidcProvider();

            oidc.RedirectURI = "i5:/";
            ServiceManager.RegisterProvider(oidc, ProviderTypes.LearningLayers);
        }

        protected override void UnRegisterServices()
        {
            ServiceManager.RemoveProvider<OpenIDConnectService>(ProviderTypes.LearningLayers);
        }
    }
}