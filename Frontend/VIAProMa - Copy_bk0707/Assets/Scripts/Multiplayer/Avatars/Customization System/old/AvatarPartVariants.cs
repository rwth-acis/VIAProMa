using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Describes and stores the differnet variants of an avatar part
/// </summary>
[CreateAssetMenu(fileName = "AvatarPartVariants", menuName = "Scriptable Objects/Setup Avatar Part Variants", order = 1)]
public class AvatarPartVariants : ScriptableObject
{
    /// <summary>
    /// Options which are available to change
    /// </summary>
    public AvatarConfigurationOption[] options;

    /// <summary>
    /// Gets the mesh of the variant with the given index
    /// </summary>
    /// <param name="variantIndex">The index of the variant</param>
    /// <returns>The mesh of the variant; if the index is out of bounds, it returns null</returns>
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

    /// <summary>
    /// Gets a material variantion of a variant
    /// </summary>
    /// <param name="variantIndex">The index of the variant</param>
    /// <param name="materialVariantIndex">The index of the material option on the variant</param>
    /// <returns>The material of the variant at the index or null if either of the two indices is out of bounds</returns>
    public Material GetMaterial(int variantIndex, int materialVariantIndex)
    {
        // no need to do anything if there are no options
        if (options.Length == 0)
        {
            return null;
        }
        // if variant index is in bounds
        if (variantIndex < options.Length)
        {
            // cannot return anything if there are no materials
            if (options[variantIndex].MaterialVariants.Length == 0)
            {
                return null;
            }
            // material index is in bounds
            if (materialVariantIndex < options[variantIndex].MaterialVariants.Length)
            {
                MaterialConfigurationOption matVar = options[variantIndex].MaterialVariants[materialVariantIndex];
                Material mat = Instantiate(matVar.Material);
                mat.color = matVar.Color;
                if (matVar.Teture != null)
                {
                    mat.mainTexture = matVar.Teture;
                }
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

    /// <summary>
    /// Gets the amount of variants for this part
    /// </summary>
    /// <returns>The length of the options array</returns>
    public int OptionsLength()
    {
        return options.Length;
    }

    /// <summary>
    /// Gets the amount of material variants for a given option
    /// </summary>
    /// <param name="optionIndex">The index of the option</param>
    /// <returns>The lenght of the material variants for an option or 0 if the option does not exist</returns>
    public int MaterialLength(int optionIndex)
    {
        if (optionIndex < options.Length)
        {
            return options[optionIndex].MaterialVariants.Length;
        }
        return 0;
    }
}
