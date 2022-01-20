using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.DeepLinkAPI;
using i5.VIAProMa.UI;
//using i5.VIAProMa.DeepLinks;

namespace i5.VIAProMa.DeepLinks
{
    /// <summary>
    /// Bootstrapper to register and unregister deep linking service as well as receiving deep links and passing them on.
    /// </summary>
    public class DeepLinkBootstrapper : BaseServiceBootstrapper
    {
        /// <summary>
        /// Registers the deep linking service and adds listener to this object.
        /// </summary>
        protected override void RegisterServices()
        {
            DeepLinkingService service = new DeepLinkingService();
            service.AddDeepLinkListener(this);
            ServiceManager.RegisterService(service);
            Debug.Log("DeepLinking Service was registered.");
        }
        /// <summary>
        /// Unregisters the deep linking service.
        /// </summary>
        protected override void UnRegisterServices()
        {
            ServiceManager.RemoveService<DeepLinkingService>();
            Debug.Log("DeepLinking Service was unregistered.");
        }
        /// <summary>
        /// Receives the deep link and passes the arguments on for processing.
        /// </summary>
        /// <param name="args">Deep link arguments.</param>
        [DeepLink(scheme: "i5", path: "invite")]
        public void ReceiveDeepLink(DeepLinkArgs args)
        {
            Debug.Log("Deep link " + args.DeepLink.AbsoluteUri + " was received.");
            StartCoroutine(DeepLinkCoroutine(args));
        }

        private IEnumerator DeepLinkCoroutine(DeepLinkArgs args)
        {
            Debug.Log("DeepLink received - pause till all instances are loaded");
            yield return null;

            Debug.Log("All Instances should be loaded - Processing DeepLink");
            DeepLinkManager.Instance.ProcessInviteDeepLink(args);
            yield return null;
        }

    }

}
