using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurationController : MonoBehaviour
{
    [SerializeField] private AvatarPart[] avatarParts;

    private SkinnedMeshRenderer partRenderer;

    private int modelIndex;
    private int materialIndex;
    private int colorIndex;

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
    public int MaterialIndex
    {
        get => materialIndex;
        set
        {
            materialIndex = value;
            colorIndex = 0;
        }
    }
    public int ColorIndex { get => colorIndex; set => colorIndex = value; }

    public AvatarPart[] AvatarParts { get => avatarParts; }

    public AvatarPart CurrentPart { get => avatarParts[modelIndex]; }

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
        for (int i=0;i<avatarParts.Length;i++)
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

    public void ApplyConfiguration()
    {
        if (ModelIndex < avatarParts.Length
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

    private void SetConfiguration(Mesh mesh, Material material, Color color)
    {
        partRenderer.sharedMesh = mesh;
        partRenderer.material = material;
        partRenderer.material.color = color;
    }
}
