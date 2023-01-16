#nullable enable

using System;
using System.Net.Http;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Threading;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

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

                HttpClient client = new HttpClient();
                string requestUri = "http://pmsl.cytexal.com:8080/resources/analytics/viaproma"; // TODO: Use correct URL
                string json = JsonConvert.SerializeObject(state);
                Response res = await Rest.PostAsync(requestUri, json)/* .GetAwaiter().GetResult() */;

                if (!res.Successful)
                    throw new Exception("Could not transmit telemetry data to backend!");
                else
                    Debug.Log(res.ResponseCode.ToString()); // TODO: Proper handeling
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