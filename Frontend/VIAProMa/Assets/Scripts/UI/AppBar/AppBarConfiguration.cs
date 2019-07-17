using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarConfiguration : MonoBehaviour
{
    public IWindow ConfigurationWindow { get; set; }

    public void ShowConfigurationWindow()
    {
        if (ConfigurationWindow != null)
        {
            ConfigurationWindow.Open();
        }
    }
}
