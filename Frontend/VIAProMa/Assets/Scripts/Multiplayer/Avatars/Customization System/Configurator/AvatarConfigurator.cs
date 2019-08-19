using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurator : MonoBehaviour
{
    [SerializeField] private GameObject avatarParent;
    [SerializeField] private VariantSelector modelSelector;
    [SerializeField] private VariantSelector materialSelector;
    [SerializeField] private VariantSelector colorSelector;

    private AvatarConfigurationController avatarConfigurationController;

    private void Awake()
    {
        if (avatarParent == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarParent));
        }
        else
        {
            FetchAvatarConfigController();
        }
    }

    private void FetchAvatarConfigController()
    {
        if (avatarConfigurationController == null)
        {
            avatarParent.GetComponentInChildren<AvatarConfigurationController>();
        }
    }

    public void SelectHairCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.HairController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.HairController.ModelIndex;

        materialSelector.Items = avatarConfigurationController.HairController.AvatarPartMaterials;
        materialSelector.SelectedIndex = avatarConfigurationController.HairController.MaterialIndex;
    }

    public void SelectGlassesCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.GlassesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.GlassesController.ModelIndex;

        materialSelector.Items = avatarConfigurationController.GlassesController.AvatarPartMaterials;
        materialSelector.SelectedIndex = avatarConfigurationController.GlassesController.MaterialIndex;
    }

    public void SelectClothesCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.ClothesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.ClothesController.ModelIndex;

        materialSelector.Items = avatarConfigurationController.ClothesController.AvatarPartMaterials;
        materialSelector.SelectedIndex = avatarConfigurationController.ClothesController.MaterialIndex;
    }
}
