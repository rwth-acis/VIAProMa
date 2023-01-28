#nullable enable

using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Newtonsoft.Json;
using i5.VIAProMa.WebConnection;

namespace VIAProMa.Assets.Scripts.Analytics
{
    public abstract class ObserverWrapper<LogType> where LogType : class
    {
        public abstract class Observer : IObserver<LogType>
        {
            private IDisposable? unsubscriber;

            public Observer(IObservable<LogType> observable)
            {
                unsubscriber = observable.Subscribe(this);
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
                string requestUri = ConnectionManager.Instance.BackendAPIBaseURL + "analytics/dummy/" + projcetID;

                Response res = await Rest.PostAsync(requestUri, json);
                ConnectionManager.Instance.CheckStatusCode(res.ResponseCode);
                string responseBody = await res.GetResponseBody();
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