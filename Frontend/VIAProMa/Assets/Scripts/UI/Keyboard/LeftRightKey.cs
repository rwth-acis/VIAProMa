using UnityEngine;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class LeftRightKey : Key
    {
        public int direction;

        protected override void KeyPressed()
        {
            base.KeyPressed();
            keyboard.CursorPos += (int)Mathf.Sign(direction);
        }
    }
}