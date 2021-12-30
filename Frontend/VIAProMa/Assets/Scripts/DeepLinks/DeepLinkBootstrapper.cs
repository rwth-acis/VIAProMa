using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.DeepLinkAPI;
//using i5.VIAProMa.DeepLinks;

namespace i5.VIAProMa.DeepLinks
{
    public class DeepLinkBootstrapper : BaseServiceBootstrapper
    {
        [SerializeField]
        private GameObject deepLinkManager;
        protected override void RegisterServices()
        {
            DeepLinkingService service = new DeepLinkingService();
            service.AddDeepLinkListener(this);
            ServiceManager.RegisterService(service);
            Debug.Log("DeepLinking Service was registered.");
        }

        protected override void UnRegisterServices()
        {
            ServiceManager.RemoveService<DeepLinkingService>();
            Debug.Log("DeepLinking Service was unregistered.");
        }

        [DeepLink("")]
        public void test(DeepLinkArgs args)
        {

            Debug.Log("--------------\n test executed !!!!!!!!!!!!!!! \n ------------------");
            Debug.Log("URI: " + args.DeepLink.AbsoluteUri);
            Debug.Log(args.Parameters.Values);
        }

        [DeepLink("test")]
        public void testtest(DeepLinkArgs args)
        {
            Debug.Log("--------------\n testtest executed !!!!!!!!!!!!!!! \n ------------------");
            Debug.Log("URI: " + args.DeepLink.AbsoluteUri);
            Debug.Log(args.Parameters.Values);
        }


        [DeepLink(scheme: "i5", path: "invite")]
        public void ReceiveDeepLink(DeepLinkArgs args)
        {
            Debug.Log("DeepLink was processed"); 
            Debug.Log("--------------\n receiveDeepLinkk executed !!!!!!!!!!!!!!! \n ------------------");
            Debug.Log("URI: " + args.DeepLink.AbsoluteUri);
            Debug.Log(args.Parameters.Values);
            ((DeepLinkManager)deepLinkManager.GetComponent("DeepLinkManager")).ProcessDeepLink(args.DeepLink.AbsoluteUri);
        }
    }

}
