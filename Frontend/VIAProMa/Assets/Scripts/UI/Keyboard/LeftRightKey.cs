using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRightKey : Key
{
    public int direction;

    protected override void KeyPressed()
    {
        base.KeyPressed();
        keyboard.CursorPos += (int)Mathf.Sign(direction);
    }
}
