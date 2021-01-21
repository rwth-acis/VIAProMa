using i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization
{
    [CreateAssetMenu(fileName = "AvatarPartMaterial", menuName = "Scriptable Objects/Avatar/Avatar Part Material", order = 1)]
    public class AvatarPartMaterial : ScriptableObject, IItem
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Color displayColor = new Color(0.6132f, 0.6132f, 0.6132f);
        [SerializeField] private Material material;
        [SerializeField] private AvatarPartColorVariations colorVariants;

        public Material Material { get => material; }

        public Color[] Colors
        {
            get
            {
                if (colorVariants == null)
                {
                    return new Color[0];
                }
                else
                {
                    return colorVariants.Colors;
                }
            }
        }

        public Color GetColor(int index)
        {
            return colorVariants.GetColor(index);
        }
        public int ColorVariationCount
        {
            get
            {
                if (colorVariants == null)
                {
                    return 0;
                }
                else
                {
                    return colorVariants.ColorVariationCount;
                }
            }
        }

        public Sprite Sprite
        {
            get => sprite;
        }

        public Color DisplayColor
        {
            get => displayColor;
        }
    }
}