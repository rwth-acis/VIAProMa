using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator
{
    public class ColorItem : IItem
    {
        public ColorItem(Color color)
        {
            DisplayColor = color;
        }

        public Sprite Sprite
        {
            get => null;
        }

        public Color DisplayColor
        {
            get; private set;
        }
    }
}