using System;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointIssueSelected : Logpoint
    {
        public string CreatorEMail { get; set; }
        public string CreatorUserName { get; set; }
        public int IssueId { get; set; }
        public string IssueName { get; set; }
        public IssueStatus Status { get; set; }

        public LogpointIssueSelected(IssueDataDisplay localDataDisplay)
        {
            this.CreatorEMail = localDataDisplay.Content.Creator.EMail;
            this.CreatorUserName = localDataDisplay.Content.Creator.UserName;
            this.IssueId = localDataDisplay.Content.Id;
            this.IssueName = localDataDisplay.Content.Name;
            this.Status = localDataDisplay.Content.Status;
        }


        public override string ToString()
        {
            return string.Format("Timestamp: {0} CreatorEMail: {1}, CreatorUserName: {2}, IssueId: {3}, IssueName: {4}, Status: {5}", Timestamp, CreatorEMail, CreatorUserName, IssueId, IssueName, Status);
        }
    }
}