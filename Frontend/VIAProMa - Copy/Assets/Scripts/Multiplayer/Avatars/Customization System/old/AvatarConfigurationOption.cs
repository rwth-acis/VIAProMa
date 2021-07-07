using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarConfigurationOption
{
    [SerializeField] private string name;
    [SerializeField] private Mesh mesh;
    [SerializeField] private MaterialConfigurationOption[] materialVariants;

    /// <summary>
    /// Name of the avatar configuration option
    /// </summary>
    public string Name { get => name; }
    /// <summary>
    /// Mesh which represents the avatar configuration option
    /// </summary>
    public Mesh Mesh { get => mesh; }
    /// <summary>
    /// Array of material variants which the configuration option offers
    /// </summary>
    public MaterialConfigurationOption[] MaterialVariants { get => materialVariants; }
}
