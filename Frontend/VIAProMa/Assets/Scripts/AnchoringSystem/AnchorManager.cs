using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer;
using UnityEngine;
using System;
using Photon.Pun;
using i5.VIAProMa.UI;

namespace i5.VIAProMa.Anchoring
{
    /// <summary>
    /// Stores and mananges the status and settings of the anchoring system
    /// </summary>
    public class AnchorManager : Singleton<AnchorManager>
    {
        // Whether or not the anchoring system is enabled
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
        /// Called to add game objects to the anchor as a child
        /// </summary>
        /// <param name="objectToAttach">The object to be attached</param>
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
