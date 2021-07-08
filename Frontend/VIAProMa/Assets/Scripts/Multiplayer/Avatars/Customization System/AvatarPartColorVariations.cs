using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization
{
    [CreateAssetMenu(fileName = "AvatarPartColorVariation", menuName = "Scriptable Objects/Avatar/Avatar Part Color Variations", order = 1)]
    public class AvatarPartColorVariations : ScriptableObject
    {
        [SerializeField] private Color[] colors;

        public Color[] Colors { get => colors; }

        public Color GetColor(int index)
        {
            return colors[index];
        }

        public int ColorVariationCount
        {
            get => colors.Length;
        }
    }
}