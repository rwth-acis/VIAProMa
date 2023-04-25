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

        /// <summary>
        /// variables storing the current status of the anchoring settings
        /// </summary>
        public bool anchorLocked = false;
        public bool anchoringEnabled = false;
        public bool moveAnchorAloneEnabled = false;

        [SerializeField] private GameObject anchorParent;
        [SerializeField] private GameObject anchorObject;
        [SerializeField] private GameObject manager;

        public GameObject AnchorObject { get => anchorObject; }
        public GameObject AnchorParent { get => anchorParent; }

        AnchoringMenu menu;

        /// <summary>
        /// Adds a game object to the anchor as a child
        /// </summary>
        public void AttachToAnchor(GameObject objectToAttach)
        {
            if (useAnchor)
            {
                //attach to anchor
                objectToAttach.transform.SetParent(anchorParent.transform);
            }
        }

        /// <summary>
        /// Completely disables anchoring and all corresponding features when leaving a room
        /// </summary>
        public void DisableAnchoring()
        {
            anchorLocked = false;
            anchoringEnabled = false;
            moveAnchorAloneEnabled = false;

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
