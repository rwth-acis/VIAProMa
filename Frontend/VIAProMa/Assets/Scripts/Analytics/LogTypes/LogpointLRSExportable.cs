namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointLRSExportable : Logpoint
    {
        public string Actor { get; set; }
        public string Verb { get; set; }
        public string ObjectType { get; set; }
        public string ObjectID { get; set; }

        public LogpointLRSExportable(string Actor, string Verb, string ObjectType, string ObjectID)
        {
            this.Actor = Actor;
            this.Verb = Verb;
            this.ObjectType = ObjectType;
            this.ObjectID = ObjectID;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Actor: {0}, Verb: {1}, ObjectType: {2}, ObjectID: {3}", Actor, Verb, ObjectType, ObjectID);
        }
    }
}