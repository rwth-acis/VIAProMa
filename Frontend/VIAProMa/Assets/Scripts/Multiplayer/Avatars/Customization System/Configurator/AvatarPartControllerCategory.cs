using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator
{
    [Serializable]
    public class AvatarPartControllerCategory
    {
        [SerializeField] private string name;
        [SerializeField] private Sprite icon;
        [SerializeField] private AvatarPartConfigurationController configurationController;

        public Sprite Icon { get => icon; }
        public string Name { get => name; }
        public AvatarPartConfigurationController ConfigurationController { get => configurationController; }
    }
}