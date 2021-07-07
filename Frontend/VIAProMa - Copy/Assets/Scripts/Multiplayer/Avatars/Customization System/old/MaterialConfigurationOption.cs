using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaterialConfigurationOption
{
    [SerializeField] private string name;
    [SerializeField] private Material material; 
    [SerializeField] private Color color;
    [SerializeField] private Texture texture;

    public string Name { get => name; }
    public Material Material { get => material; }
    public Color Color { get => color; }
    public Texture Teture { get => texture; }
}
