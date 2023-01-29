using System;

namespace VIAProMa.Assets.Scripts.ProjectSettings
{
    [Serializable]
    public class ProjectSettings
    {
        public bool IsAnalyticsEnabled { set; get; }

        // Add other settings by including a public Property (you can use AutoProperty as in the example with the setting AnalyticsEnabled).
    }
}