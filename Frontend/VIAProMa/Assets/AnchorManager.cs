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

        private GameObject anchorParent;
        private GameObject anchorObject;

        private bool useAnchor = true;

        /// <summary>
        /// variables storing the corrent status of the anchoring settings
        /// </summary>
        public bool anchorLocked = false;
        public bool anchoringEnabled = false;
        public bool moveAnchorAloneEnabled = false;

        [SerializeField] private GameObject anchor;
        [SerializeField] private GameObject manager;

        AnchoringMenu menu;

        /// <summary>
        /// Adds a game object to the anchor as a child
        /// </summary
        public void AttachToAnchor(GameObject objectToAttach)
        {
            if (useAnchor)
            {
                //attach to anchor
                objectToAttach.transform.SetParent(anchor.transform);
            }
        }

        /// <summary>
        /// Completely disables anchoring and all corresponding features when leaving a room
        /// </summary
        public void DisableAnchoring()
        {
            anchorLocked = false;
            anchoringEnabled = false;
            moveAnchorAloneEnabled = false;

            anchorParent = this.transform.parent.parent.gameObject;
            anchorObject = anchorParent.transform.Find("AnchorObject").gameObject;
            anchorObject.SetActive(false);

            menu = manager.GetComponentInChildren<AnchoringMenu>();
            if(menu != null)
            {
                menu.DisableAnchoring();
                menu.DisableAnchorLock();
                menu.DisableMoveAnchorAlone();
                menu.Close();
            }        
        }
    }
}