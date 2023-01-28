using i5.Toolkit.Core.ExperienceAPI;
using i5.Toolkit.Core.OpenIDConnectClient;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointLRSExportable : Logpoint
    {
        public Actor Actor { get; set; }
        public Verb Verb { get; set; }
        public XApiObject Object { get; set; }

        public LogpointLRSExportable(string VerbID, string ObjectIRI)
        {
            // Get the login information of the current user if they are logged in to their account (currently GitHub and RequirementsBaazar) for issue management. If the user is not logged in, this will be placeholder data.
            IUserInfo userInfo = AnalyticsManager.Instance.UserInfo;

            string userEmail = "mailto:" + userInfo.Email;
            string userName = userInfo.Username;

            // Initialize the statement's Actor, Verb and Object.
            this.Actor = new Actor(userEmail, userName);
            this.Verb = new Verb(VerbID);
            this.Object = new XApiObject(ObjectIRI);
            this.Object.type = "http://activitystrea.ms/schema/1.0/issue";
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Actor Email: {0}, VerbID: {1}, ObjectID: {2}", Actor.Mbox, Verb.id, Object.id);
        }

        public Statement GetStatement()
        {
            return new Statement(this.Actor, this.Verb, this.Object);
        }
    }
}