using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using VIAProMa.Assets.Scripts.Analytics;

namespace i5.VIAProMa.UI
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

        public Text textElement;

        [SerializeField] public Interactable AnalyticsToggleBtn;

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

        private void Update()
        {
            AnalyticsToggleBtn.IsToggled = AnalyticsManager.Instance.AnalyticsEnabled;
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

        public void ToggleAnalytics()
        {
            AnalyticsManager.Instance.AnalyticsEnabled = AnalyticsToggleBtn.IsToggled;
        }
    }
}