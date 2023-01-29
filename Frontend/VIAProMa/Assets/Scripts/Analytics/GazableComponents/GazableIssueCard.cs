using System.Collections.Generic;
using UnityEngine;
using System;
using VIAProMa.Assets.Scripts.Analytics.LogTypes;
using i5.VIAProMa.SaveLoadSystem.Core;

namespace VIAProMa.Assets.Scripts.Analytics.GazableComponents
{
    public class GazableIssueCard : AnalyticsObservibleComponent<LogpointGazedAt>
    {
        public void GazeDetected()
        {
            string loggedObjectID = this.GetComponent<Serializer>().Id;
            LogpointGazedAt logpoint = new LogpointGazedAt(loggedObjectID, "IssueCard");
            NotifyObservers(logpoint);
        }

        protected override void CreateObservers()
        {
            _ = new Observer<LogpointGazedAt>(this);
        }
    }
}
