using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the customization of an avatar
/// </summary>
[RequireComponent(typeof(SkinnedMeshRenderer))]
public class CustomizationController : MonoBehaviour
{
    /// <summary>
    /// Part varians of the avatar
    /// </summary>
    public AvatarPartVariants partVariants;

    private SkinnedMeshRenderer skinnedRenderer;

    /// <summary>
    /// Currently selected variant index
    /// </summary>
    public int VariantIndex { get; private set; }
    /// <summary>
    /// Currently selected material variant index
    /// </summary>
    public int MaterialVariantIndex { get; private set; }

    /// <summary>
    /// Initializes the controller
    /// </summary>
    private void Awake()
    {
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        if (partVariants != null)
        {
            SetVariant(0, 0);
        }
    }

    /// <summary>
    /// Sets the currently displayed variant and material variant
    /// </summary>
    /// <param name="variantIndex">The index of the selected variant</param>
    /// <param name="materialVariantIndex">The material index of the selected material variant</param>
    public void SetVariant(int variantIndex, int materialVariantIndex)
    {
        skinnedRenderer.sharedMesh = partVariants.GetMesh(variantIndex);
        skinnedRenderer.material = partVariants.GetMaterial(variantIndex, materialVariantIndex);
        VariantIndex = variantIndex;
        MaterialVariantIndex = materialVariantIndex;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MaterialVariantIndex++;
            int materialLength = partVariants.MaterialLength(VariantIndex);
            if (MaterialVariantIndex == materialLength || materialLength == 0)
            {
                MaterialVariantIndex = 0;
                VariantIndex++;
                if (VariantIndex == partVariants.OptionsLength())
                {
                    VariantIndex = 0;
                }
            }
            SetVariant(VariantIndex, MaterialVariantIndex);
        }
    }
}
