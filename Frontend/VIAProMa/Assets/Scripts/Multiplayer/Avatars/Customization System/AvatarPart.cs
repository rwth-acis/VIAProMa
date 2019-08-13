﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPart", menuName = "Scriptable Objects/Avatar/Avatar Part", order = 1)]
public class AvatarPart : ScriptableObject
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private AvatarPartMaterial[] avatarPartMaterials;

    public Mesh Mesh { get => mesh; }

    public AvatarPartMaterial GetAvatarPartMaterial(int index)
    {
        return avatarPartMaterials[index];
    }

    public int MaterialVariationCount { get => avatarPartMaterials.Length; }

    public Color GetMaterialColorVariation(int materialIndex, int colorIndex)
    {
        return avatarPartMaterials[materialIndex].GetColor(colorIndex);
    }

    public int GetMaterialColorVariationCount(int materialIndex)
    {
        return avatarPartMaterials[materialIndex].ColorVariationCount;
    }
}
