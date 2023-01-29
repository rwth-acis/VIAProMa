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
    public abstract class Observer<LogType> : IObserver<LogType> where LogType : Logpoint
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

        public abstract void OnNext(LogType state);

        public void Unsubscribe()
        {
            if (unsubscriber == null)
                throw new Exception("Cannot unsubscribe from observing, as the observer has not been subscribed to any observable yet!");
            else
                unsubscriber.Dispose();
        }
    }
}