using System;
using UnityEngine;
using i5.VIAProMa.UI;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.DeepLinks.InviteLinks;
using TMPro;

namespace i5.VIAProMa.DeepLinks.InviteLinks
{
    public class InviteLinkMenu : MonoBehaviour, IWindow
    {

        [Header("UI Elements")]
        [SerializeField] private TextMeshPro linkTextfield;


        public bool WindowEnabled { get; set; } // not used here

        public bool WindowOpen { get; private set; }

        public event EventHandler WindowClosed;

        private string curInviteLink;

        private void Awake()
        {
            if (linkTextfield == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(linkTextfield));
            }
        }

        public void OnEnable()
        {
            curInviteLink = InviteLinkManager.generateInviteLink();
            linkTextfield.text = curInviteLink;

        }

        /// <summary>
        /// Opens the window by making the GameObject active
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
            WindowOpen = true;
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        /// <summary>
        /// Closes the window and raises the WindowClosed event
        /// Deactivates the GameObject (so the window still exists but is invisible)
        /// </summary>
        public void Close()
        {
            WindowOpen = false;
            WindowClosed?.Invoke(this, EventArgs.Empty);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Copys the invite link into the clipboard
        /// </summary>
        public void CopyLinkToClipboard() 
        {
            GUIUtility.systemCopyBuffer = curInviteLink;
        }
    }
}


