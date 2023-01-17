using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointGazedAt : Logpoint
    {
        public string ProjectName { get; set; }
        public string LogID { get; set; }
        public string LoggedType { get; set; }

        public LogpointGazedAt(string ProjectName, string LogID, string LoggedType)
        {
            this.ProjectName = ProjectName;
            this.LogID = LogID;
            this.LoggedType = LoggedType;
        }

        public override string ToString()
        {
            return string.Format("{0}: Project Name: {1}, LogID: {2} LoggedType: {3}", base.Timestamp, ProjectName, LogID, LoggedType);
        }
    }
}