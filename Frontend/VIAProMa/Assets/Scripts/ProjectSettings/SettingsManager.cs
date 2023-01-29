using System.Threading.Tasks;
using Newtonsoft.Json;
using HoloToolkit.Unity;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun;
using i5.VIAProMa.WebConnection;
using VIAProMa.Assets.Scripts.Analytics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace VIAProMa.Assets.Scripts.ProjectSettings
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        private ProjectSettings _settings;

        /// <summary>
        /// Whether analytics (telemetry data) should be collected or not.
        /// </summary>
        public bool IsAnalyticsEnabled
        {
            get { return _settings.IsAnalyticsEnabled; }
            set
            {
                // Let other players pull the new settings.
                _settings.IsAnalyticsEnabled = value;
                PhotonView.Get(this).RPC("SetIsAnalyticsEnabled", RpcTarget.Others, value);

                // Make all players display the popup with the new analytics enabled state.
                PhotonView.Get(this).RPC("ShowAnalyticsPopup", RpcTarget.All);

                SetSettingsOnBackend();
            }
        }

        public SettingsManager()
        {
            // Instantiate the local settings with the default settings for VIAProMa projects.
            _settings = new ProjectSettings();
        }


        #region AnalyticsPopup
        // Everything concerned with the popup notification when the setting IsAnalyticsEnabled is changed.
        public Text TextObject;
        public GameObject Background;
        public AudioSource NotificationSound;


        /// <summary>
        /// Shows the blue popup to the current player/client with the information about whether telemetry is enabled. This method can (besides usual calls) be called via RPCs.
        /// </summary>
        [PunRPC]
        public IEnumerator ShowAnalyticsPopup()
        {
            TextObject.text = SettingsManager.Instance.IsAnalyticsEnabled ? "Analytics Enabled!" : "Analytics Disabled!";
            NotificationSound.Play();

            Background.SetActive(true);
            TextObject.enabled = true;
            yield return new WaitForSeconds(3);
            Background.SetActive(false);
            TextObject.enabled = false;
        }
        #endregion

        public async Task GetSettingsFromBackendAsync()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();
            Response resp =
                    await Rest.GetAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "project-settings/" + projectID,
                        null,
                        -1,
                        null,
                        true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            string responseBody = await resp.GetResponseBody();

            // Make sure to display popup for the analytics enabled state (again), if the corresponding setting changed.
            bool wereAnalyticsEnabled = IsAnalyticsEnabled;

            _settings = JsonConvert.DeserializeObject<ProjectSettings>(responseBody);

            // If the IsAnalyticsEnabled setting changed, display the popup.
            if (_settings.IsAnalyticsEnabled != wereAnalyticsEnabled)
                StartCoroutine(SettingsManager.Instance.ShowAnalyticsPopup());
        }

        private async void SetSettingsOnBackend()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();
            string settingsJSON = JsonConvert.SerializeObject(_settings);
            Response resp =
                    await Rest.PostAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "project-settings/" + projectID, settingsJSON);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        }

        /// <summary>
        /// This method gets executed via RPC if any client changed IsAnalyticsEnabled so that all other clients can update their setting as well.
        /// </summary>
        [PunRPC]
        private void SetIsAnalyticsEnabled(bool enabled)
        {
            _settings.IsAnalyticsEnabled = enabled;
        }
    }
}