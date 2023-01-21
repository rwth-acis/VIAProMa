using System;
using HoloToolkit.Unity;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Utilities;
using i5.VIAProMa.WebConnection;
using UnityEngine;
using Newtonsoft.Json;
using Microsoft.MixedReality.Toolkit;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private AnalyticsSettings _settings;
    private Guid _projectGUID;

    [SerializeField]
    public bool AnalyticsEnabled
    {
        get { return _settings.AnalyticsEnabled; }
        set
        {
            PhotonView.Get(this).RPC("SetAnalyticsEnabled", RpcTarget.Others, value);
            _settings.AnalyticsEnabled = value;
            SetSettingsOnBackend();
        }
    }

    public Guid ProjectID
    {
        get { return _projectGUID; }
        set { _projectGUID = value; }
    }


    public AnalyticsManager()
    {
        _settings = new AnalyticsSettings();
    }

    public void Start()
    {
        GetSettingsFromBackendAsync();
        if (_settings.AnalyticsEnabled && !CoreServices.InputSystem.EyeGazeProvider.IsEyeTrackingEnabled)
        {
            Debug.LogError("Eye tracking is disabled! Instuctions: " +
                "https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/features/input/eye-tracking/eye-tracking-basic-setup?view=mrtkunity-2022-05#testing-your-unity-app-on-a-hololens-2");
        }

        // Generate a new GUID for the VIAProMa project that the analytics refer to. If the project ID is already set (for example because the project has been saved and loaded), do nothing and use the old ID.
        if (_projectGUID.Equals(Guid.Empty))
            _projectGUID = Guid.NewGuid();
        Debug.Log(_projectGUID);
    }

    private async void GetSettingsFromBackendAsync()
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
    }
}
