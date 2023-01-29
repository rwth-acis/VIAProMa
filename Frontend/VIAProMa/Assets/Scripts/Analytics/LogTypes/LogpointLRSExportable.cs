using i5.Toolkit.Core.ExperienceAPI;
using i5.Toolkit.Core.OpenIDConnectClient;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointLRSExportable : Logpoint
    {
        public string Actor { get; set; }
        public string Verb { get; set; }
        public string ObjectType { get; set; }
        public string ObjectID { get; set; }

        public LogpointLRSExportable(string verbID, string objectType, string objectURI)
        {
            Actor = AnalyticsManager.Instance.UserInfo.Username;
            Verb = verbID;
            ObjectType = objectType;
            ObjectID = objectURI;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Actor Username: {0}, VerbID: {1}, ObjectID: {2}", Actor, Verb, ObjectID);
        }

        public Statement GetStatement()
        {
            // Get the login information of the current user if they are logged in to their account (currently GitHub and RequirementsBaazar) for issue management. If the user is not logged in, this will be placeholder data.
            IUserInfo userInfo = AnalyticsManager.Instance.UserInfo;
            string userEmail = "mailto:" + userInfo.Email;
            string userName = userInfo.Username;

            // Initialize the statement's Actor, Verb and Object.
            Actor actor = new Actor(userEmail, userName);
            Verb verb = new Verb(Verb);
            XApiObject obj = new XApiObject(ObjectID);
            obj.type = ObjectType;
            return new Statement(actor, verb, obj);
        }
    }
}