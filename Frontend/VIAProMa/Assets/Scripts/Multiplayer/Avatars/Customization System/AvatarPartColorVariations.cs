using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPartColorVariation", menuName = "Scriptable Objects/Avatar/Avatar Part Color Variations", order = 1)]
public class AvatarPartColorVariations : ScriptableObject
{
    [SerializeField] private Color[] colors;

    public Color GetColor(int index)
    {
        return colors[index];
    }

    public int ColorVariationCount
    {
        get => colors.Length;
    }
}
