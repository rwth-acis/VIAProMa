using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarPartControllerCategory
{
    [SerializeField] private Sprite icon;
    [SerializeField] private AvatarPartConfigurationController configurationController;

    public Sprite Icon { get => icon; }
    public AvatarPartConfigurationController ConfigurationController { get => configurationController; }
}
