using System;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public abstract class Logpoint
    {
        public DateTime Timestamp { get; set; }

        public Logpoint()
        {
            this.Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            return Timestamp.ToString();
        }
    }
}