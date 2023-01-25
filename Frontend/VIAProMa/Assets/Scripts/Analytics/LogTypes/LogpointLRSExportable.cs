namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public class LogpointLRSExportable : Logpoint
    {
        public string Actor { get; set; }
        public string Verb { get; set; }
        public string Object { get; set; }

        public LogpointLRSExportable(string Actor, string Verb, string Object)
        {
            this.Actor = Actor;
            this.Verb = Verb;
            this.Object = Object;
        }

        public override string ToString()
        {
            return base.ToString() + string.Format(", Actor: {0}, Verb: {1}, Object: {2}", Actor, Verb, Object);
        }
    }
}