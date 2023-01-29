using System;
using System.Collections.Generic;

using UnityEngine;
using VIAProMa.Assets.Scripts.ProjectSettings;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public abstract class AnalyticsObservibleComponent<LogType> : MonoBehaviour, IObservable<LogType> where LogType : Logpoint
    {
        protected List<IObserver<LogType>> Observers { get; private set; }

        protected virtual void Awake()
        {
            Observers = new List<IObserver<LogType>>();
            CreateObservers();
        }

        public IDisposable Subscribe(IObserver<LogType> observer)
        {
            if (!Observers.Contains(observer))
                Observers.Add(observer);
            return new Unsubscriber(Observers, observer);
        }

        protected void NotifyObservers(LogType log)
        {
            if (SettingsManager.Instance.IsAnalyticsEnabled)
                foreach (var observer in Observers)
                    observer.OnNext(log);
        }

        protected abstract void CreateObservers();

        // This class must be implemented to enable an Observer to unsubscribe from an Observable.
        // The interface IObservable<...> requires the return type to implement the IDisposable interface which is satisfied by the Unsubscriber class.
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<LogType>> _observers;
            private IObserver<LogType> _observer;

            public Unsubscriber(List<IObserver<LogType>> observers, IObserver<LogType> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}