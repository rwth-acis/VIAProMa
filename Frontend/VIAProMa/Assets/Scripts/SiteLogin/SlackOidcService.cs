using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.Utilities;

namespace i5.VIAProMa.Login
{
    public class SlackOidcService : OpenIDConnectService
    {
        /// <summary>
        /// Opens a login page in the system's default browser so that the user can log in
        /// Requires a configured OpenID Connect provider
        /// </summary>
        public void SlackOpenLoginPage()
        {
            if (OidcProvider == null)
            {
                i5Debug.LogError("OIDC provider is not set. Please set the OIDC provider before accessing the OIDC workflow.", this);
                return;
            }

            // for all non-native apps: create a listener server
#if UNITY_EDITOR || UNITY_STANDALONE

            if (ServerListener == null)
            {
                i5Debug.LogError("Redirect server listener is not set. Please set it before accessing the OIDC workflow.", this);
                return;
            }

            if (ServerListener.ServerActive)
            {
                string[] arrUri = ServerListener.ListeningUri.Split(':');
                ServerListener.ListeningUri = arrUri[0] + ":" + arrUri[1];
                OidcProvider.OpenLoginPage(Scopes, ServerListener.ListeningUri);
            }
            else
            {
                if (string.IsNullOrEmpty(ServerListener.ListeningUri))
                {
                    ServerListener.GenerateListeningUri();
                }
                string urlStart = "<html><head>";
                string customAdditionalRedirect = "";
                if (!string.IsNullOrEmpty(RedirectURI))
                {
                    customAdditionalRedirect = string.Format("<meta http-equiv=\"Refresh\" content=\"0; url = {0}\" />"
                        , RedirectURI);
                }
                string urlEnd = "</head><body>Please return to the app</body></html>";
                ServerListener.ResponseString = urlStart + customAdditionalRedirect + urlEnd;
                ServerListener.RedirectReceived += ServerListener_RedirectReceived;
                ServerListener.StartServer();

                string[] arrUri = ServerListener.ListeningUri.Split(':');
                ServerListener.ListeningUri = arrUri[0] + ":" + arrUri[1];
                OidcProvider.OpenLoginPage(Scopes, ServerListener.ListeningUri);
            }

            // for native apps use deep linking
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WSA
            OidcProvider.OpenLoginPage(Scopes, RedirectURI);
            
            // unsupported platforms get an error message
#else
            throw new PlatformNotSupportedException("This platform is not supported.");
#endif
        } 
    } 
}