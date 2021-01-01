using UnityEngine;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class KeySetSwitchKey : Key
    {
        [SerializeField] private int targetPageIndex;

        protected override void KeyPressed()
        {
            base.KeyPressed();
            keyboard.CurrentKeySetPageIndex = targetPageIndex;
        }
    }
}