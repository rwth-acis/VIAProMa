using i5.VIAProMa.UI;
using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.WebConnection;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer
{
    /// <summary>
    /// Controls the UI for the advanced settings menu.
    /// This menu allows to configure analytical settings.
    /// </summary>
    public class AdvancedSettingsMenu : MonoBehaviourPunCallbacks, IWindow
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject advancedSettingsMenu;

        public bool WindowOpen { get; private set; }
        public bool WindowEnabled { get; set; } // not used here

        public event EventHandler WindowClosed;

        /// <summary>
        /// Initializes the component and makes sure that it is set up correctly
        /// </summary>
        private void Awake()
        {
            if (advancedSettingsMenu == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(advancedSettingsMenu));
            }
            else
            {
                advancedSettingsMenu.SetActive(false);
            }
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
    }
}