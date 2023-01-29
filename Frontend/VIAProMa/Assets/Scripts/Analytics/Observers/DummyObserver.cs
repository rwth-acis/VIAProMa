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
    /// <summary>
    /// DummyObserver logs generic <see cref="Logpoint">s into json Files on the VIAProMa backend.
    /// This Observer is for testing purposes only and the logged data is not part of the analytics database.
    /// </summary>
    public class DummyObserver : Observer<Logpoint>
    {
        public DummyObserver(IObservable<Logpoint> observable) : base(observable)
        {
        }

        public override async void OnNext(Logpoint state)
        {
            if (!SettingsManager.Instance.IsAnalyticsEnabled)
            {
                Debug.LogWarning("Telemetry is disabled!"); // TODO: improve
                return;
            }

            // Make call to REST API to log state to file
            string projcetID = AnalyticsManager.Instance.ProjectID.ToString();
            string json = JsonConvert.SerializeObject(state);
            string requestUri = ConnectionManager.Instance.BackendAPIBaseURL + "analytics/dummy/" + projcetID;

            Response res = await Rest.PostAsync(requestUri, json);

            if (!res.Successful)
                throw new Exception("Could not transmit telemetry data to backend!");
        }
    }
}