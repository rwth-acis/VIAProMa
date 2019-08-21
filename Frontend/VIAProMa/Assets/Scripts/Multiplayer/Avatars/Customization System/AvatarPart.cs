using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPart", menuName = "Scriptable Objects/Avatar/Avatar Part", order = 1)]
public class AvatarPart : ScriptableObject, IItem
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Color displayColor = new Color(0.6132f, 0.6132f, 0.6132f);
    [SerializeField] private Mesh mesh;
    [SerializeField] private AvatarPartMaterial[] avatarPartMaterials;

    public Mesh Mesh { get => mesh; }

    public AvatarPartMaterial[] PartMaterials
    {
        get => avatarPartMaterials;
    }

    public AvatarPartMaterial GetAvatarPartMaterial(int index)
    {
        if (index >= avatarPartMaterials.Length)
        {
            return null;
        }
        else
        {
            return avatarPartMaterials[index];
        }
    }

    public int MaterialVariationCount { get => avatarPartMaterials.Length; }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Color DisplayColor
    {
        get => displayColor;
    }

    public Color GetMaterialColorVariation(int materialIndex, int colorIndex)
    {
        return avatarPartMaterials[materialIndex].GetColor(colorIndex);
    }

    public int GetMaterialColorVariationCount(int materialIndex)
    {
        return avatarPartMaterials[materialIndex].ColorVariationCount;
    }
}
