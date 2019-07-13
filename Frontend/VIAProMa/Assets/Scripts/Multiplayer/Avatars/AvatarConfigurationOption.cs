using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AvatarConfigurationOption
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;

    public Mesh Mesh { get => mesh; }
    public Material Material { get => material; }
}
