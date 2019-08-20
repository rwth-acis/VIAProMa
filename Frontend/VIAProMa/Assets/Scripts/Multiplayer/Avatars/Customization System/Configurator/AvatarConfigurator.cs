using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarConfigurator : MonoBehaviour
{
    [SerializeField] private GameObject avatarParent;
    [SerializeField] private InteractableToggleCollection categoryToggles;
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private VariantSelector modelSelector;
    [SerializeField] private VariantSelector materialSelector;
    [SerializeField] private VariantSelector colorSelector;

    private GridObjectCollection categoryObjectCollection;
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

        if (categoryToggles == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(categoryToggles));
        }
        else
        {
            categoryObjectCollection = categoryToggles.gameObject.GetComponent<GridObjectCollection>();
            if (categoryObjectCollection == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(GridObjectCollection), gameObject);
            }
        }
        if (categoryButtonPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(categoryButtonPrefab));
        }
        if (modelSelector == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(modelSelector));
        }
        if (materialSelector == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(materialSelector));
        }
        if (colorSelector == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorSelector));
        }

        InitializeCategories();

        modelSelector.ItemSelected += ModelItemSelected;
        materialSelector.ItemSelected += MaterialItemSelected;
    }

    private void InitializeCategories()
    {
        List<Interactable> categoryInteractables = new List<Interactable>();
        for (int i=0;i<avatarConfigurationController.AvatarPartControllers.Length;i++)
        {
            GameObject categoryBtnInstance = Instantiate(categoryButtonPrefab, categoryToggles.transform);
            Interactable categoryBtn = categoryBtnInstance.GetComponent<Interactable>();
            categoryBtn.OnClick.AddListener(OnCategorySelection);
            SpriteRenderer iconRenderer = categoryBtnInstance.GetComponentInChildren<SpriteRenderer>();
            iconRenderer.sprite = avatarConfigurationController.AvatarPartControllers[i].Icon;
            categoryInteractables.Add(categoryBtn);
        }
        categoryObjectCollection.UpdateCollection();
        categoryToggles.ToggleList = categoryInteractables.ToArray();
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
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].ConfigurationController;
        modelSelector.Items = selectedPartController.AvatarParts;
        modelSelector.SelectedIndex = selectedPartController.ModelIndex;

        materialSelector.Items = selectedPartController.AvatarPartMaterials;
        materialSelector.SelectedIndex = selectedPartController.MaterialIndex;
    }

    private void ModelItemSelected(object sender, EventArgs e)
    {
        EnsureAvatarConfigController();
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].ConfigurationController;
        selectedPartController.ModelIndex = modelSelector.SelectedIndex;
        selectedPartController.ApplyConfiguration();
        materialSelector.Items = selectedPartController.AvatarPartMaterials;
    }

    private void MaterialItemSelected(object sender, EventArgs e)
    {
        EnsureAvatarConfigController();
        AvatarPartConfigurationController selectedPartController = avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].ConfigurationController;
        selectedPartController.MaterialIndex = materialSelector.SelectedIndex;
        selectedPartController.ApplyConfiguration();
    }
}
