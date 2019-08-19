﻿using Microsoft.MixedReality.Toolkit.UI;
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
        EnsureAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.HairController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.HairController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.HairController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.HairController.MaterialIndex;
    }

    public void SelectGlassesCategory()
    {
        EnsureAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.GlassesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.GlassesController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.GlassesController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.GlassesController.MaterialIndex;
    }

    public void SelectClothesCategory()
    {
        EnsureAvatarConfigController();
        modelSelector.Items = avatarConfigurationController.ClothesController.AvatarParts;
        modelSelector.SelectedIndex = avatarConfigurationController.ClothesController.ModelIndex;

        //materialSelector.Items = avatarConfigurationController.ClothesController.AvatarPartMaterials;
        //materialSelector.SelectedIndex = avatarConfigurationController.ClothesController.MaterialIndex;
    }

    private void ModelItemSelected(object sender, EventArgs e)
    {
        EnsureAvatarConfigController();
        switch(categoryCollection.CurrentIndex)
        {
            case 0:
                avatarConfigurationController.HairController.ModelIndex = modelSelector.SelectedIndex;
                avatarConfigurationController.HairController.ApplyConfiguration();
                break;
            case 1:
                avatarConfigurationController.GlassesController.ModelIndex = modelSelector.SelectedIndex;
                avatarConfigurationController.GlassesController.ApplyConfiguration();
                break;
            case 2:
                avatarConfigurationController.ClothesController.ModelIndex = modelSelector.SelectedIndex;
                avatarConfigurationController.ClothesController.ApplyConfiguration();
                break;
            default:
                Debug.LogError("Avatar Config UI accessed category which is out of bounds.", gameObject);
                break;
        }
    }
}
