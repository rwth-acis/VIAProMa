using System.Globalization;
using System.Text.RegularExpressions;
using i5.Toolkit.Core.ExperienceAPI;
using UnityEngine;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointLRSExportable : Logpoint
    {
        public Actor Actor { get; set; }
        public Verb Verb { get; set; }
        public XApiObject Object { get; set; }

        public LogpointLRSExportable(string VerbID, string ObjectIRI)
        {
            string rawEmail = GetUserEmail();
            string userEmail = "mailto:" + rawEmail;

            // Extract the user name from the email by selecting everything before the @ symbol and replacing dots (.) with spaces. After that capitalize the first letter in each word.
            TextInfo ti = new CultureInfo("de-DE", false).TextInfo;
            string userName = new Regex(@"([\w_\-\.]*)@.*").Match(rawEmail).Groups[1].Value.Replace(".", " ");
            userName = ti.ToTitleCase(userName);

            this.Actor = new Actor(userEmail, userName);
            this.Verb = new Verb(VerbID);
            this.Object = new XApiObject(ObjectIRI);
            this.Object.type = "http://activitystrea.ms/schema/1.0/issue";
            Debug.LogError(this.Object.id + "     " + this.Object.type);
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Actor: {0}, Verb: {1}, Object: {2}", Actor.Mbox, Verb.id, Object.id);
        }

        public Statement GetStatement()
        {
            return new Statement(this.Actor, this.Verb, this.Object);
        }

        private string GetUserEmail()
        {
            // Get the email of the current user if they are logged in to their account for issue management. Else return a anonymous placeholder email.
            // TODO: Get email if user is logged in.
            return "anonymoususer@viaproma.com";
        }
    }
}