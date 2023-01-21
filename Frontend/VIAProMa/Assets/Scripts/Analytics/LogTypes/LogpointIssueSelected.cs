using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    // TODO: Maybe reduce information down to Issue ID, to be consistent with LogpointGazedAt
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

            this.CreatorEMail ??= "No Email provided";
            this.CreatorUserName ??= "No CreatorUserName provided";
            if (this.IssueId == 0)
                this.IssueId = -1;
            this.IssueName ??= "No IssueName provided";
            // TODO: Proper error handeling for missing Status is required!
        }


        public override string ToString()
        {
            return base.ToString() + string.Format(", Creator EMail: {0}, Creator User Name: {1}, Issue ID: {2}, Issue Name: {3}, Status: {4}", CreatorEMail, CreatorUserName, IssueId, IssueName, Status);
        }
    }
}