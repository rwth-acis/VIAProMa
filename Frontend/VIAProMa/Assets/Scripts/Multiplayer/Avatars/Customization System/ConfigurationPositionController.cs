using i5.VIAProMa.Utilities;
using System;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization
{
    public class ConfigurationPositionController : MonoBehaviour, IConfigurationController
    {
        [SerializeField] private Vector3[] positionsForAvatars;

        private int avatarIndex;

        public int AvatarIndex
        {
            get => avatarIndex;
            set
            {
                bool configChanged = avatarIndex != value;
                avatarIndex = value;
                if (configChanged)
                {
                    ConfigurationChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public int ModelIndex { get; set; }
        public int MaterialIndex { get; set; }
        public int ColorIndex { get; set; }

        public event EventHandler ConfigurationChanged;

        private void Awake()
        {
            if (positionsForAvatars.Length == 0)
            {
                SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(positionsForAvatars));
            }
        }

        public void ApplyConfiguration()
        {
            if (positionsForAvatars.Length == 0)
            {
                return;
            }

            int selectedIndex = AvatarIndex;
            if (selectedIndex >= positionsForAvatars.Length || selectedIndex < 0)
            {
                selectedIndex = 0;
            }
            transform.localPosition = positionsForAvatars[selectedIndex];
        }
    }
}