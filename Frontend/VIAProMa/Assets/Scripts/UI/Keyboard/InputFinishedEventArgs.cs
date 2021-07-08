using System;

namespace i5.VIAProMa.UI.KeyboardInput
{
    public class InputFinishedEventArgs : EventArgs
    {
        public bool Aborted { get; set; }
        public string Text { get; set; }
    }

    public delegate void InputFinishedEventHandler(object sender, InputFinishedEventArgs e);
}