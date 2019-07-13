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

    public string Name { get => name; }
    public Mesh Mesh { get => mesh; }
    public MaterialConfigurationOption[] MaterialVariants { get => materialVariants; }
}
