using Microsoft.MixedReality.Toolkit.UI;
using System;
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
            EnsureAvatarConfigController();
        }

        if (categoryCollection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(categoryCollection));
        }

        modelSelector.ItemSelected += ModelItemSelected;
        materialSelector.ItemSelected += MaterialItemSelected;
    }

    private void Start()
    {
        OnCategorySelection();
    }

    private void EnsureAvatarConfigController()
    {
        if (avatarConfigurationController == null)
        {
            avatarConfigurationController = avatarParent.GetComponentInChildren<AvatarConfigurationController>();
        }
    }

    public void OnCategorySelection()
    {
        EnsureAvatarConfigController();
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryCollection.CurrentIndex].ConfigurationController;
        modelSelector.Items = selectedPartController.AvatarParts;
        modelSelector.SelectedIndex = selectedPartController.ModelIndex;

        materialSelector.Items = selectedPartController.AvatarPartMaterials;
        materialSelector.SelectedIndex = selectedPartController.MaterialIndex;
    }

    private void ModelItemSelected(object sender, EventArgs e)
    {
        EnsureAvatarConfigController();
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryCollection.CurrentIndex].ConfigurationController;
        selectedPartController.ModelIndex = modelSelector.SelectedIndex;
        selectedPartController.ApplyConfiguration();
        materialSelector.Items = selectedPartController.AvatarPartMaterials;
    }

    private void MaterialItemSelected(object sender, EventArgs e)
    {
        EnsureAvatarConfigController();
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryCollection.CurrentIndex].ConfigurationController;
        selectedPartController.MaterialIndex = materialSelector.SelectedIndex;
        selectedPartController.ApplyConfiguration();
    }
}
