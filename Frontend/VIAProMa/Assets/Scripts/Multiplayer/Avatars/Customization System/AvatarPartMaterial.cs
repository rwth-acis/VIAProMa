using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPartMaterial", menuName = "Scriptable Objects/Avatar/Avatar Part Material", order = 1)]
public class AvatarPartMaterial : ScriptableObject, IItem
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private Material material;
    [SerializeField] private AvatarPartColorVariations colorVariants;

    public Material Material { get => material; }

    public Color[] Colors { get => colorVariants.Colors; }

    public Color GetColor(int index)
    {
        return colorVariants.GetColor(index);
    }
    public int ColorVariationCount
    {
        get
        {
            if (colorVariants == null)
            {
                return 0;
            }
            else
            {
                return colorVariants.ColorVariationCount;
            }
        }
    }

    public Sprite Sprite
    {
        get => sprite;
    }
}
