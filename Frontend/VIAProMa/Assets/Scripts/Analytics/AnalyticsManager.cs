using HoloToolkit.Unity;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Utilities;
using i5.VIAProMa.WebConnection;
using UnityEngine;
using Newtonsoft.Json;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using UnityEngine.UI;
using System;
using POpusCodec.Enums;
using UnityEngine.TextCore.Text;
using UnityEngine.XR.ARSubsystems;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private AnalyticsSettings _settings;

    public Text TextObject;
    public DateTime CurrentAt;
    public DateTime ExpiresAt;

    [SerializeField]
    public bool AnalyticsEnabled {
        
        get { return _settings.AnalyticsEnabled; } 
        set {
            PhotonView.Get(this).RPC("SetAnalyticsEnabled", RpcTarget.Others, value);
            _settings.AnalyticsEnabled = value;

            ShowEnabledText(); //TODO: Photon Implementation
            SetSettingsOnBackend();
        }
    }

    public void ShowEnabledText()
    {
        TextObject.text = "Telemetry Enabled!";
        if (AnalyticsManager.Instance.AnalyticsEnabled)
        {
            CurrentAt = DateTime.Now;
            ExpiresAt = DateTime.Now.AddSeconds(5);
        }
    }
    public void Update()
    {
        CurrentAt = DateTime.Now;
        if (AnalyticsManager.Instance.AnalyticsEnabled && (CurrentAt < ExpiresAt)) TextObject.enabled = true;
        else TextObject.enabled = false;  
    }

    public AnalyticsManager()
    {
        _settings = new AnalyticsSettings();
    }

    public void Start()
    {
        TextObject.text = "";
        ShowEnabledText();
        GetSettingsFromBackendAsync();
        if (_settings.AnalyticsEnabled && !CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled)
        {
            Debug.LogError("Eye tracking is disabled! Instuctions: " +
                "https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/eye-tracking/eye-tracking-basic-setup?view=mrtkunity-2022-05#testing-your-unity-app-on-a-hololens-2");
        }
    }

    private async void GetSettingsFromBackendAsync()
    {
        ShowEnabledText();
        Response resp =
                await Rest.GetAsync(
                    ConnectionManager.Instance.BackendAPIBaseURL + "analytics/viaproma/settings",
                    null,
                    -1,
                    null,
                    true);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        string responseBody = await resp.GetResponseBody();
        _settings = JsonConvert.DeserializeObject<AnalyticsSettings>(responseBody);
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
    private void SetAnalyticsEnabled(bool enabled)
    {
        _settings.AnalyticsEnabled = enabled;
        ShowEnabledText();
    }
}
