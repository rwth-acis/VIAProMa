using System;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointGazedAt : Logpoint
    {
        public string LoggedObjectID { get; set; }
        public string LoggedObjectType { get; set; }

        public LogpointGazedAt(string LoggedObjectID, string LoggedObjectType)
        {
            this.LoggedObjectID = LoggedObjectID;
            this.LoggedObjectType = LoggedObjectType;
        }

        public override string ToString()
        {
            return string.Format("{0}: VIAProMaProjectID: {1}, LoggedObjectID: {2} LoggedObjectType: {3}", base.Timestamp, VIAProMaProjectID, LoggedObjectID, LoggedObjectType);
        }
    }
}