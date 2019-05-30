using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputFinishedEventArgs : EventArgs
{
    public bool Aborted { get; set; }
    public string Text { get; set; }
}
