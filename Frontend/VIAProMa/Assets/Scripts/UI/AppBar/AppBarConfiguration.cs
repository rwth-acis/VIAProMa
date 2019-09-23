using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for an app bar with a configuration button
/// </summary>
public class AppBarConfiguration : MonoBehaviour
{
    /// <summary>
    /// The configuration window which should be shown if the configuration button is pressed
    /// </summary>
    public IWindow ConfigurationWindow { get; set; }

    /// <summary>
    /// Shows the configuration window
    /// This method should be called by the configuration button on the app bar
    /// </summary>
    public void ShowConfigurationWindow()
    {
        if (ConfigurationWindow != null)
        {
            ConfigurationWindow.Open();
        }
    }
}
