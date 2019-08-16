using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for windows
/// </summary>
public interface IWindow
{
    /// <summary>
    /// Event which is invoked if the window is closed
    /// </summary>
    event EventHandler WindowClosed;

    /// <summary>
    /// Gets or sets the enabled status of the window; if disabled, all UI elements should also be disabled
    /// </summary>
    bool WindowEnabled { get; set; }

    /// <summary>
    /// Opens the window
    /// </summary>
    void Open();

    /// <summary>
    /// Closes the window
    /// </summary>
    void Close();
}
