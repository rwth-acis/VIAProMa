#nullable enable

using System;
using UnityEngine;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;

namespace VIAProMa.Assets.Scripts.Analytics
{
    /// <summary>
    /// Abstract base class for reporting analytics data to endpoints.
    /// All Observers must implement <see cref="OnNext"/>, which is called by the Observers <see cref="System.IObservable{T}"/>
    /// </summary>
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

        /// <summary>
        /// OnNext is called by the Observers <see cref="System.IObservable{T}"/>.
        /// This function should use the given <see cref="Logpoint"/> to log the observed analytics event.
        /// </summary>
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