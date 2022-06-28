using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer;
using UnityEngine;
using System;
using Photon.Pun;
using i5.VIAProMa.UI;

namespace i5.VIAProMa.Anchoring
{
    public class AnchorManager : Singleton<AnchorManager>
    {
        private bool useAnchor = true;
        [SerializeField] private GameObject anchor;
        [SerializeField] private GameObject manager;

        AnchoringMenu menu;
  

        public void AttachToAnchor(GameObject objectToAttach)
        {
            if (useAnchor)
            {
                //attach to anchor
                objectToAttach.transform.SetParent(anchor.transform);
            }
        }

        public void DisableAnchoring()
        {
            menu = manager.GetComponentInChildren<AnchoringMenu>();
            menu.DisableAnchoring();
            menu.Close();
        }
    }
}