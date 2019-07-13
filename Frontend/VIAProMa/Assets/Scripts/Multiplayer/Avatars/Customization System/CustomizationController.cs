using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class CustomizationController : MonoBehaviour
{
    public AvatarPartVariants partVariants;

    private SkinnedMeshRenderer skinnedRenderer;

    public int VariantIndex { get; private set; }
    public int MaterialVariantIndex { get; private set; }

    private void Awake()
    {
        skinnedRenderer = GetComponent<SkinnedMeshRenderer>();
        if (partVariants != null)
        {
            SetVariant(0, 0);
        }
    }

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
