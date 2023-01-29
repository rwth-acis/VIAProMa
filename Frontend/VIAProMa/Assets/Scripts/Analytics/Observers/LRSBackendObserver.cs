#nullable enable

using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using i5.VIAProMa.WebConnection;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using VIAProMa.Assets.Scripts.ProjectSettings;

namespace VIAProMa.Assets.Scripts.Analytics
{
    public class LRSBackendObserver : Observer<LogpointLRSExportable>
    {

        public LRSBackendObserver(IObservable<LogpointLRSExportable> observable) : base(observable)
        {
        }


        public override async void OnNext(LogpointLRSExportable state)
        {
            if (!SettingsManager.Instance.IsAnalyticsEnabled)
            {
                Debug.LogWarning("Telemetry is disabled!"); // TODO: improve
                return;
            }

            // Make call to REST API to log state to file
            string projcetID = AnalyticsManager.Instance.ProjectID.ToString();
            string json = JsonConvert.SerializeObject(state);
            string requestUri = ConnectionManager.Instance.BackendAPIBaseURL + "analytics/lrs/" + projcetID;

            Response res = await Rest.PostAsync(requestUri, json);

            if (!res.Successful)
                throw new Exception("Could not transmit telemetry data to backend!");
        }
    }
}