using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackKey : Key
{
    protected override void KeyPressed()
    {
        base.KeyPressed();
        keyboard.Backspace();
    }
}
