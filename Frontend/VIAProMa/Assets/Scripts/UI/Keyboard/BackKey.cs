namespace i5.VIAProMa.UI.KeyboardInput
{
    public class BackKey : Key
    {
        protected override void KeyPressed()
        {
            base.KeyPressed();
            keyboard.Backspace();
        }
    }
}