#nullable enable

using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using i5.VIAProMa.WebConnection;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using i5.Toolkit.Core.ExperienceAPI;
using i5.Toolkit.Core.Utilities;

namespace VIAProMa.Assets.Scripts.Analytics
{
    public abstract class ObserverWrapper<LogType> where LogType : class
    {
        public abstract class Observer : IObserver<LogType>
        {
            private IDisposable? unsubscriber;
            private ExperienceAPIClient lrsClient;

            public Observer(IObservable<LogType> observable)
            {
                unsubscriber = observable.Subscribe(this);

                Uri lrsEndpoint = new Uri("https://lrs.tech4comp.dbis.rwth-aachen.de/data/xAPI");
                string authToken = "a218e7ee16b48874f25428c5b790d1014c741da3";
                lrsClient = new ExperienceAPIClient();
                lrsClient.XApiEndpoint = lrsEndpoint;
                lrsClient.AuthorizationToken = authToken;
            }

            // Subscribe to another observable than the one subscribed to previously.
            public void Subscribe(IObservable<LogType> provider)
            {
                if (unsubscriber != null)
                    unsubscriber.Dispose();
                else
                    unsubscriber = provider.Subscribe(this);
            }

            public virtual void OnCompleted()
            {
                // Probably not needed? Maybe make call to api to finish logging or something.
                this.Unsubscribe();
            }

            public virtual void OnError(Exception e)
            {
                // TODO: Decide individually whether to silently continue or to write error to log file, the make call to api.
                Debug.Log(e.Message);
            }

            public async void OnNext(LogType state)
            {
                // TODO: Check again whether telemetry is enabled to disable logging even when the check is omitted in the user's NotifyObservers method
                if (!AnalyticsManager.Instance.AnalyticsEnabled)
                {
                    Debug.LogWarning("Telemetry is disabled!"); // TODO: improve
                    return;
                }

                // Make call to REST API to log state to file
                string projcetID = AnalyticsManager.Instance.ProjectID.ToString();
                string json = JsonConvert.SerializeObject(state);
                string requestUri = ConnectionManager.Instance.BackendAPIBaseURL + "projects/analytics/" + projcetID;

                Response res = await Rest.PostAsync(requestUri, json);

                if (!res.Successful)
                    throw new Exception("Could not transmit telemetry data to backend!");
                else
                    Debug.Log(res.ResponseCode.ToString()); // TODO: Proper handeling

                if (state is LogpointLRSExportable)
                {
                    // Make request to LRS.
                    LogpointLRSExportable? lrsState = state as LogpointLRSExportable;
                    Actor actor = new Actor(lrsState!.Actor);
                    Verb verb = new Verb(lrsState.Verb);
                    XApiObject apiObject = new XApiObject(lrsState.ObjectID);

                    WebResponse<string> lrsRes = await lrsClient.SendStatementAsync(actor, verb, apiObject);
                    Debug.Log(lrsRes.Content);
                }
            }

            public void Unsubscribe()
            {
                if (unsubscriber == null)
                    throw new Exception("Cannot unsubscribe from observing, as the observer has not been subscribed to any observable yet!");
                else
                    unsubscriber.Dispose();
            }
        }
    }
}