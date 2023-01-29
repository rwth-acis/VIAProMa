#nullable enable

using System;
using UnityEngine;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using i5.Toolkit.Core.ExperienceAPI;
using i5.Toolkit.Core.Utilities;


namespace VIAProMa.Assets.Scripts.Analytics
{
    /// <summary>
    /// LRSObserver is an <see cref="Observer{Logtype}"> for logging <see cref="LogpointLRSExportable"> events.
    /// It sends the data to a Learning Record Store (LRS)
    /// </summary>
    public class LRSObserver : Observer<LogpointLRSExportable>
    {
        private ExperienceAPIClient lrsClient;

        public LRSObserver(IObservable<LogpointLRSExportable> observable) : base(observable)
        {
            Uri lrsEndpoint = new Uri("https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI");
            lrsClient = new ExperienceAPIClient();
            lrsClient.XApiEndpoint = new Uri("https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI/");
            lrsClient.AuthorizationToken = "Basic MDkwOGNhNGFmMTRjNzg3YWU2MjIzODUwMDIxYWMwNjRkYTc5MDcwMDphMjE4ZTdlZTE2YjQ4ODc0ZjI1NDI4YzViNzkwZDEwMTRjNzQxZGEz"; // TODO: Change key when publishing.
            lrsClient.Version = "1.0.3";
        }


        public override async void OnNext(LogpointLRSExportable state)
        {
            if (!AnalyticsManager.Instance.AnalyticsEnabled)
            {
                Debug.LogWarning("Telemetry is disabled!"); // TODO: improve
                return;
            }

            // At the time of commiting, there is a bug in the i5 toolkit, because of which the next line will always fail. We assume that the code will work when it is fixed. See https://github.com/rwth-acis/i5-Toolkit-for-Unity/issues/21
            LogpointLRSExportable? lrsState = state as LogpointLRSExportable;
            WebResponse<string> lrsRes = await lrsClient.SendStatementAsync(lrsState!.GetStatement());
        }
    }
}