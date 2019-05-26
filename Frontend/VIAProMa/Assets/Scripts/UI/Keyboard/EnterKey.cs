using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterKey : Key
{
    protected override void KeyPressed()
    {
        base.KeyPressed();
        keyboard.Text += Environment.NewLine;
    }
}
