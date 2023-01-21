using HoloToolkit.Unity;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Utilities;
using i5.VIAProMa.WebConnection;
using UnityEngine;
using Newtonsoft.Json;
using Microsoft.MixedReality.Toolkit;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private AnalyticsSettings _settings;

    public Text TextObject;
    public GameObject Background;
    public DateTime CurrentAt;
    public DateTime ExpiresAt;
    private bool isStartOver = false;

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
            PhotonView.Get(this).RPC("ShowIsTelemetryEnabledPopup", RpcTarget.Others, value);

            SetSettingsOnBackend();
        }
    }

    public void ShowIsTelemetryEnabledPopup()
    {

        if (AnalyticsManager.Instance.AnalyticsEnabled)
        {
            TextObject.text = "Telemetry Enabled!";
        }
        else
        {
            TextObject.text = "Telemetry Disabled!";
        }

        CurrentAt = DateTime.Now;
        ExpiresAt = DateTime.Now.AddSeconds(2.5);
    }

    public void Update()
    {
        CurrentAt = DateTime.Now;
        if ((CurrentAt < ExpiresAt) && isStartOver)
        {
            Background.SetActive(true);
            TextObject.enabled = true;
        }
        else
        {
            Background.SetActive(false);
            TextObject.enabled = false;
        }
    }

    public AnalyticsManager()
    {
        _settings = new AnalyticsSettings();
    }

    public async Task Start()
    {
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
        Response resp =
                await Rest.GetAsync(
                    ConnectionManager.Instance.BackendAPIBaseURL + "analytics/viaproma/settings",
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
        string settingsJSON = JsonConvert.SerializeObject(_settings);
        Response resp =
                await Rest.PostAsync(
                    ConnectionManager.Instance.BackendAPIBaseURL + "analytics/viaproma/settings", settingsJSON);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
    }

    [PunRPC]
    private void SetIsAnalyticsEnabled(bool enabled)
    {
        this.AnalyticsEnabled = enabled;
    }
}
