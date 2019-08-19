using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurator : MonoBehaviour
{
    [SerializeField] private GameObject avatarParent;
    [SerializeField] private InteractableToggleCollection categoryCollection;
    [SerializeField] private VariantSelector modelSelector;
    [SerializeField] private VariantSelector materialSelector;
    [SerializeField] private VariantSelector colorSelector;

    private AvatarConfigurationController avatarConfigurationController;
    private AvatarSpineController spineController;

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

        if (categoryCollection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(categoryCollection));
        }
    }

    private void FetchAvatarConfigController()
    {
        if (avatarConfigurationController == null)
        {
            avatarConfigurationController = avatarParent.GetComponentInChildren<AvatarConfigurationController>();
        }
    }

    public void OnCategorySelection()
    {
        switch(categoryCollection.CurrentIndex)
        {
            case 0:
                SelectHairCategory();
                break;
            case 1:
                SelectGlassesCategory();
                break;
            case 2:
                SelectClothesCategory();
                break;
            default:
                Debug.LogError("Avatar Config UI accessed category which is out of bounds.", gameObject);
                break;
        }
    }

    public void SelectHairCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.HairController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.HairController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.HairController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.HairController.MaterialIndex;
    }

    public void SelectGlassesCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.GlassesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.GlassesController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.GlassesController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.GlassesController.MaterialIndex;
    }

    public void SelectClothesCategory()
    {
        FetchAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.ClothesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.ClothesController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.ClothesController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.ClothesController.MaterialIndex;
    }
}
