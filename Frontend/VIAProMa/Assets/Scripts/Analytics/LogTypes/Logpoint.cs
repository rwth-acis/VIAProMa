using System;
using Photon.Pun;

namespace VIAProMa.Assets.Scripts.Analytics.LogTypes
{
    public abstract class Logpoint
    {
        public DateTime Timestamp { get; set; }
        public Guid VIAProMaProjectID { get; set; }
        public string UserId { get; set; }

        public Logpoint()
        {
            this.Timestamp = DateTime.Now;
            this.VIAProMaProjectID = AnalyticsManager.Instance.ProjectID;
            this.UserId = PhotonNetwork.LocalPlayer.UserId;
        }

        public override string ToString()
        {
            return Timestamp.ToString();
        }
    }
}