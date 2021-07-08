using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConfigurationController
{
    int AvatarIndex { get; set; }
    int ModelIndex { get; set; }
    int MaterialIndex { get; set; }
    int ColorIndex { get; set; }

    event EventHandler ConfigurationChanged;

    void ApplyConfiguration();
}
