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
        public void ProcessDeepLink(string deepLink)
        {
            Debug.Log("Process Deep Link was reached - URI: " + deepLink);
        }
    }
}

