using i5.VIAProMa.Analytics.FileExport;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System;
using UnityEngine;
using VIAProMa.Assets.Scripts.Analytics.FileExport;
using VIAProMa.Assets.Scripts.ProjectSettings;

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
        [SerializeField] private DropdownCapability exportFormatSelection;

        public bool WindowOpen { get; private set; }
        public bool WindowEnabled { get; set; } // not used here

        public event EventHandler WindowClosed;

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
            AnalyticsToggleBtn.IsToggled = SettingsManager.Instance.IsAnalyticsEnabled;
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
            SettingsManager.Instance.IsAnalyticsEnabled = AnalyticsToggleBtn.IsToggled;
        }
        public void ExportAnalyticsData()
        {
            ExportAnalytics.Instance.ExportAsync(exportFormatSelection.ChosenExportSelection);
        }
    }
}