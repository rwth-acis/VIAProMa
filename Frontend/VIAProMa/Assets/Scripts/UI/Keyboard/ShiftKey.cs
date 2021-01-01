using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class ShiftKey : Key, IShiftableKey
    {
        [SerializeField] private GameObject shiftIndicator;

        protected override void Awake()
        {
            base.Awake();
            if (shiftIndicator == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(shiftIndicator));
                return;
            }

            SetShift(false);

        }

        protected override void KeyPressed()
        {
            base.KeyPressed();
            keyboard.ShiftActive = !keyboard.ShiftActive;
        }

        public void SetShift(bool shiftActive)
        {
            shiftIndicator.SetActive(shiftActive);
        }
    }
}