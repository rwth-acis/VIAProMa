using System;
using UnityEngine;

namespace i5.VIAProMa.UI
{
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

        bool WindowOpen { get; }

        /// <summary>
        /// Opens the window
        /// </summary>
        void Open();

        void Open(Vector3 position, Vector3 eulerAngles);

        /// <summary>
        /// Closes the window
        /// </summary>
        void Close();
    }
}