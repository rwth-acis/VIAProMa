using i5.Toolkit.Core.Utilities;
using System;

namespace i5.Toolkit.Core.OpenIDConnectClient
{
    /// <summary>
    /// Data description of the answer that is received after the access token was requested in the
    /// authorization flow
    /// </summary>
    [Serializable]
    public class SlackAuthorizationFlowAnswer
    {
        public string app_id;
        public SlackTeam team;
        public SlackAuthUser authed_user;
    }

    [Serializable]
    public class SlackAuthUser
    {
        public string access_token;
        public string scope;
        public string token_type;

    }

    [Serializable]
    public class SlackTeam
    {
        public string id;
    }
}
