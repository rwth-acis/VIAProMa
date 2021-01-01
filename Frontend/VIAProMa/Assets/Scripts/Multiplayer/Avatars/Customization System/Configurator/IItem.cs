using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator
{
    public interface IItem
    {
        Sprite Sprite { get; }

        Color DisplayColor { get; }
    }
}