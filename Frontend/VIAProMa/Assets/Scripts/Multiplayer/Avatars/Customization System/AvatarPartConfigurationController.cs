using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the configuration of one single avatar part
/// </summary>
public class AvatarPartConfigurationController : MonoBehaviour
{
    [Tooltip("The available options for this part")]
    [SerializeField] private AvatarPart[] avatarParts;

    private SkinnedMeshRenderer partRenderer;

    private int modelIndex;
    private int materialIndex;
    private int colorIndex;

    /// <summary>
    /// The index of the 3D model which is displayed on this part
    /// </summary>
    public int ModelIndex
    {
        get => modelIndex;
        set
        {
            modelIndex = value;
            materialIndex = 0;
            colorIndex = 0;
        }
    }

    /// <summary>
    /// The index of the material which is displayed on this part
    /// </summary>
    public int MaterialIndex
    {
        get => materialIndex;
        set
        {
            materialIndex = value;
            colorIndex = 0;
        }
    }

    /// <summary>
    /// The index of the color variation which is displayed on this part
    /// </summary>
    public int ColorIndex { get => colorIndex; set => colorIndex = value; }

    /// <summary>
    /// The available avatar part options for this part
    /// </summary>
    public AvatarPart[] AvatarParts { get => avatarParts; }

    /// <summary>
    /// The availalbe avatar part materials for the currently displayed model
    /// </summary>
    public AvatarPartMaterial[] AvatarPartMaterials { get => avatarParts[modelIndex].PartMaterials; }

    /// <summary>
    /// The available color variations for the currently displayed model and material
    /// </summary>
    public Color[] AvatarPartColors { get => avatarParts[modelIndex].GetAvatarPartMaterial(materialIndex).Colors; }

    /// <summary>
    /// The currently displayed part 3D model
    /// </summary>
    public AvatarPart CurrentPart { get => avatarParts[modelIndex]; }

    /// <summary>
    /// The curerntly displayed part material
    /// </summary>
    public AvatarPartMaterial CurrentMaterial { get => avatarParts[modelIndex].GetAvatarPartMaterial(materialIndex); }

    /// <summary>
    /// The currently displayed color variation
    /// </summary>
    public Color CurrentColor { get => avatarParts[modelIndex].GetAvatarPartMaterial(materialIndex).GetColor(colorIndex); }

    /// <summary>
    /// Initializes the component, checks the setup
    /// </summary>
    private void Awake()
    {
        // check setup
        partRenderer = GetComponent<SkinnedMeshRenderer>();
        if (partRenderer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(partRenderer), gameObject);
        }
        if (avatarParts.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(avatarParts));
        }
        for (int i = 0; i < avatarParts.Length; i++)
        {
            if (avatarParts[i] == null)
            {
                SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(avatarParts), i);
            }
            else if (avatarParts[i].MaterialVariationCount == 0)
            {
                SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, "MaterialVariationCount");
            }
        }
    }

    /// <summary>
    /// Applies the selected configuration indices to the renderer on the avatar
    /// </summary>
    public void ApplyConfiguration()
    {
        // handle case that no material variation was given (e.g. for the case "do not show this part")
        if (avatarParts[ModelIndex].Mesh == null && avatarParts[ModelIndex].MaterialVariationCount == 0)
        {
            SetConfiguration(null, null, Color.white);
        }
        // otherwise set the part's appearance according to the indices
        else if (ModelIndex < avatarParts.Length
            && MaterialIndex < avatarParts[ModelIndex].MaterialVariationCount)
        {
            AvatarPartMaterial avatarMat = avatarParts[ModelIndex].GetAvatarPartMaterial(MaterialIndex);
            Color avatarMatColor = Color.white; // avatar part does not necessarily have color variations => use white in this case

            if (ColorIndex < avatarMat.ColorVariationCount)
            {
                avatarMatColor = avatarMat.GetColor(ColorIndex);
            }

            SetConfiguration(avatarParts[ModelIndex].Mesh, avatarMat.Material, avatarMatColor);
        }
    }

    /// <summary>
    /// Applies a specific (already processed and extracted) configuration to the avatar
    /// </summary>
    /// <param name="mesh">The mesh to show</param>
    /// <param name="material">The material which is applied to the mesh's renderer</param>
    /// <param name="color">The color which is set on the material's _color parameter</param>
    private void SetConfiguration(Mesh mesh, Material material, Color color)
    {
        partRenderer.sharedMesh = mesh;
        partRenderer.material = material;
        partRenderer.material.color = color;
    }
}
