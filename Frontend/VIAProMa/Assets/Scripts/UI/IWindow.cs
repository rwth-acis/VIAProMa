using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWindow
{
    event EventHandler WindowClosed;

    bool WindowEnabled { get; set; }

    void Open();
    void Close();
}
