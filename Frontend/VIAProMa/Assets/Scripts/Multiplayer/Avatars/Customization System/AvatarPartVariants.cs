using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AvatarPartVariants", menuName = "Scriptable Objects/Setup Avatar Part Variants", order = 1)]
public class AvatarPartVariants : ScriptableObject
{
    public AvatarConfigurationOption[] options;

    public Mesh GetMesh(int variantIndex)
    {
        if (variantIndex < options.Length)
        {
            return options[variantIndex].Mesh;
        }
        else
        {
            Debug.LogError("Requested mesh for avatar configuration outside of array bounds");
            return null;
        }
    }

    public Material GetMaterial(int variantIndex, int materialVariantIndex)
    {
        if (options.Length == 0)
        {
            return null;
        }
        if (variantIndex < options.Length)
        {
            if (options[variantIndex].MaterialVariants.Length == 0)
            {
                return null;
            }
            if (materialVariantIndex < options[variantIndex].MaterialVariants.Length)
            {
                MaterialConfigurationOption matVar = options[variantIndex].MaterialVariants[materialVariantIndex];
                Material mat = matVar.Material;
                mat.color = matVar.Color;
                mat.mainTexture = matVar.Teture;
                return mat;
            }
            else
            {
                Debug.LogError("Requested avatar configuration material outside of array bounds");
            }
        }
        else
        {
            Debug.LogError("Requested material for avatar configuration outside of array bounds");
        }
        return null;
    }

    public int OptionsLength()
    {
        return options.Length;
    }

    public int MaterialLength(int optionIndex)
    {
        if (optionIndex < options.Length)
        {
            return options[optionIndex].MaterialVariants.Length;
        }
        return 0;
    }
}
