using HoloToolkit.Unity;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit.Utilities;
using i5.VIAProMa.WebConnection;
using UnityEngine;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private bool _analyticsEnabled;
    public bool AnalyticsEnabled { get { return _analyticsEnabled; } set { PhotonView.Get(this).RPC("setAnalyticsEnabled", RpcTarget.All, value); } }

    public AnalyticsManager()
    {
        CheckAnalyticsFromBackendAsync();
    }

    private async void CheckAnalyticsFromBackendAsync()
    {
        // _analyticsEnabled = true;
        Response resp =
                await Rest.GetAsync(
                    ConnectionManager.Instance.BackendAPIBaseURL + "analytics/test2/settings",
                    null,
                    -1,
                    null,
                    true);
        ConnectionManager.Instance.CheckStatusCode(resp.ResponseCode);
        string responseBody = await resp.GetResponseBody();
        _analyticsEnabled = responseBody == "true";
    }

    [PunRPC]
    private void SetAnalyticsEnabled(bool enabled)
    {
        _analyticsEnabled = enabled;
    }
}
