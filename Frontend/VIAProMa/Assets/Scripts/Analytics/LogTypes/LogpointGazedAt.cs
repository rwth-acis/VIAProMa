namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointGazedAt : Logpoint
    {
        public string ProjectName { get; set; }
        public string LogID { get; set; }
        public string LoggedObjectType { get; set; }

        public LogpointGazedAt(string ProjectName, string LogID, string LoggedObjectType)
        {
            this.ProjectName = ProjectName;
            this.LogID = LogID;
            this.LoggedObjectType = LoggedObjectType;
        }

        public override string ToString()
        {
            return string.Format("{0}: Project Name: {1}, LogID: {2} LoggedType: {3}", base.Timestamp, ProjectName, LogID, LoggedObjectType);
        }
    }
}