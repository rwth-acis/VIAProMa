using System;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization
{
    public interface IConfigurationController
    {
        int AvatarIndex { get; set; }
        int ModelIndex { get; set; }
        int MaterialIndex { get; set; }
        int ColorIndex { get; set; }

        event EventHandler ConfigurationChanged;

        void ApplyConfiguration();
    }
}