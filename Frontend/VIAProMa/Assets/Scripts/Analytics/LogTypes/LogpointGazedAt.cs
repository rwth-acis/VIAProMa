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
            return base.ToString() + string.Format(", Logged Object ID: {0} Logged Object Type: {1}", LoggedObjectID, LoggedObjectType);
        }
    }
}