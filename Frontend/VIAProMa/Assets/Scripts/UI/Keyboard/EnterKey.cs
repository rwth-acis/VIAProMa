using System;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class EnterKey : Key
    {
        protected override void KeyPressed()
        {
            base.KeyPressed();
            keyboard.Text += Environment.NewLine;
        }
    }
}