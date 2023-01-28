using System;
using HoloToolkit.Unity;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Utilities;
using i5.VIAProMa.WebConnection;
using UnityEngine;
using Newtonsoft.Json;
using Microsoft.MixedReality.Toolkit;
using System.Threading.Tasks;
using UnityEngine.UI;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.Login;

namespace VIAProMa.Assets.Scripts.Analytics
{
    public class AnalyticsManager : Singleton<AnalyticsManager>
    {
        private AnalyticsSettings _settings;
        private Guid _projectGUID;
        private IUserInfo _userInfo;

        #region Everything concerned with the Notification PopUp
        public Text TextObject;
        public GameObject Background;
        public DateTime ExpiresAt;
        private bool isStartOver = false;
        public AudioSource NotificationSound;
        #endregion Everything concerned with the Notification PopUp

        [SerializeField]
        public bool AnalyticsEnabled
        {
            get { return _settings.AnalyticsEnabled; }
            set
            {
                // Let other players pull the new settings.
                PhotonView.Get(this).RPC("SetIsAnalyticsEnabled", RpcTarget.Others, value);
                _settings.AnalyticsEnabled = value;

                ShowIsTelemetryEnabledPopup();
                // Let other players display the new telemetry enabled state popup.
                PhotonView.Get(this).RPC("ShowIsTelemetryEnabledPopup", RpcTarget.Others);

                SetSettingsOnBackend();
            }
        }

        public Guid ProjectID
        {
            get { return _projectGUID; }
            set { _projectGUID = value; }
        }

        public IUserInfo UserInfo { get; set; }

        public void Update()
        {
            bool showPopup = (DateTime.Now < ExpiresAt) && isStartOver;
            Background.SetActive(showPopup);
            TextObject.enabled = showPopup;
        }

        [PunRPC]
        public void ShowIsTelemetryEnabledPopup()
        {
            TextObject.text = AnalyticsManager.Instance.AnalyticsEnabled ? "Telemetry Enabled!" : "Telemetry Disabled!";
            ExpiresAt = DateTime.Now.AddSeconds(2.5);
            NotificationSound.Play();
        }

        public AnalyticsManager()
        {
            _settings = new AnalyticsSettings();
        }

        public async Task Start()
        {
            // Initialize data about the current user as anonymous (user is neither logged into GitHub nor the RequirementsBazaar).
            SetUserAnonymous();

            // Generate a new GUID for the VIAProMa project that the analytics refer to. If the project ID is already set (for example because the project has been saved and loaded), do nothing and use the old ID.
            if (_projectGUID.Equals(Guid.Empty))
                _projectGUID = Guid.NewGuid();
            Debug.Log("This Project has the ID: " + AnalyticsManager.Instance.ProjectID);

            await GetSettingsFromBackendAsync();
            if (_settings.AnalyticsEnabled && !CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled)
            {
                Debug.LogError("Eye tracking is disabled! Instuctions: " +
                    "https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/eye-tracking/eye-tracking-basic-setup?view=mrtkunity-2022-05#testing-your-unity-app-on-a-hololens-2");
            }

            TextObject.text = "";
            ShowIsTelemetryEnabledPopup();
            isStartOver = true;
        }

        private async Task GetSettingsFromBackendAsync()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();
            Response resp =
                    await Rest.GetAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "projects/settings/" + projectID,
                        null,
                        -1,
                        null,
                        true);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
            string responseBody = await resp.GetResponseBody();
            _settings = JsonConvert.DeserializeObject<AnalyticsSettings>(responseBody); // TODO: Edit to only get Analytics enabled key, when updating to project based analytics.
        }

        private async void SetSettingsOnBackend()
        {
            string projectID = AnalyticsManager.Instance.ProjectID.ToString();
            string settingsJSON = JsonConvert.SerializeObject(_settings);
            Response resp =
                    await Rest.PostAsync(
                        ConnectionManager.Instance.BackendAPIBaseURL + "projects/settings/" + projectID, settingsJSON);
            ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        }

        [PunRPC]
        private void SetIsAnalyticsEnabled(bool enabled)
        {
            _settings.AnalyticsEnabled = enabled;
        }

        public async Task FetchLearningLayersUserDataFromServiceManager()
        {
            IUserInfo userInfo = await ServiceManager.GetService<LearningLayersOidcService>().GetUserDataAsync();
            if (userInfo != null)
                this.UserInfo = userInfo;
        }

        public void SetUserAnonymous()
        {
            this.UserInfo = new LearningLayersUserInfo("AnonymousUser", "anonymoususer@viaproma.com", "Anonymous User");
        }
    }
}