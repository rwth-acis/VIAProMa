using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.DeepLinkAPI;
using i5.VIAProMa.UI.MainMenuCube;

namespace i5.VIAProMa.DeepLinks
{
    /// <summary>
    /// Deep Link Manager which handles the reception of Deep Links at runtime
    /// </summary>
    public class DeepLinkManager : Singleton<DeepLinkManager>
    {
        /// <summary>
        /// Processes an invite deep link.
        /// </summary>
        /// <param name="args">Deep link arguments.</param>
        public void ProcessInviteDeepLink(DeepLinkArgs args)
        {
            Debug.Log("DeepLink " + args + " is being processed.");
            Dictionary<string, string> parameters = args.Parameters;
            InviteLinksHandler.Instance.JoinByDeepLink(parameters);
        }
    }
}

