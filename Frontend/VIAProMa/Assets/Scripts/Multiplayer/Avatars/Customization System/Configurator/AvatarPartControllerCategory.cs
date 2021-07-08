using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarPartControllerCategory
{
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [SerializeField] private AvatarPartConfigurationController configurationController;

    public Sprite Icon { get => icon; }
    public string Name { get => name; }
    public AvatarPartConfigurationController ConfigurationController { get => configurationController; }
}
