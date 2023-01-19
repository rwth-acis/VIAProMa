using System.Collections.Generic;
using UnityEngine;
using System;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using VIAProMa.Assets.Scripts.Analytics;
using i5.VIAProMa.UI.ListView.Issues;

namespace VIAProMa.Assets.Scripts.Analytics.GazableComponents
{
    public class GazableIssueCard : MonoBehaviour, IObservable<LogpointGazedAt>
    {
        private List<IObserver<LogpointGazedAt>> observers = new List<IObserver<LogpointGazedAt>>();

        void Awake()
        {
            _ = new IssueCardGazingObserver(this);
        }

        public void GazeDetected()
        {
            string id = this.GetComponent<IssueDataDisplay>().Content.Id.ToString();
            Debug.Log("ID des Issues" + id);
            LogpointGazedAt logpoint = new LogpointGazedAt("VIAProMa Project Name", id, "IssueCard");  //TODO: Fill with data!
            NotifyObservers(logpoint);
        }

        // Implementation of necessary methods for the analytics module
        #region Observer Implemenations
        public IDisposable Subscribe(IObserver<LogpointGazedAt> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        public void NotifyObservers(LogpointGazedAt log)
        {
            if (AnalyticsManager.Instance.AnalyticsEnabled)
                foreach (var observer in observers)
                    observer.OnNext(log);
        }


        // This class must be implemented to enable an Observer to unsubscribe from an Observable.
        // The interface IObservable<...> requires the return type to implement the IDisposable interface which is satisfied by the Unsubscriber class.
        private class Unsubscriber : IDisposable
        {
            private List<IObserver<LogpointGazedAt>> _observers;
            private IObserver<LogpointGazedAt> _observer;

            public Unsubscriber(List<IObserver<LogpointGazedAt>> observers, IObserver<LogpointGazedAt> observer)
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
        class IssueCardGazingObserver : ObserverWrapper<LogpointGazedAt>.Observer
        {
            public IssueCardGazingObserver(IObservable<LogpointGazedAt> observable) : base(observable) { }
        }
        #endregion Observer Implemenations
    }
}
