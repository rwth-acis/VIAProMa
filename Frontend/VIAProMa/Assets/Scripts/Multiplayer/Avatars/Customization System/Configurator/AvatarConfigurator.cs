using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Multiplayer.Avatars.Customization.Configurator
{
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

        private GameObject UndoRedoManagerGameObject;
        private UndoRedoManager UndoRedoManager;

        private void Awake()
        {
            if (avatarParent == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(avatarParent));
            }
            else
            {
                EnsureAvatarConfigController();
                // deactivate the spine controller
                spineController = avatarParent.GetComponentInChildren<AvatarSpineController>();
                spineController.enabled = false;
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
            colorSelector.ItemSelected += ColorItemSelected;

            UndoRedoManagerGameObject = GameObject.Find("UndoRedo Manager");
            UndoRedoManager = UndoRedoManagerGameObject.GetComponent<UndoRedoManager>();
        }

        private void InitializeCategories()
        {
            List<Interactable> categoryInteractables = new List<Interactable>();
            for (int i = 0; i < avatarConfigurationController.AvatarPartControllers.Length; i++)
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
            categoryToggles.enabled = true; // enable the category toggles now to avoid initialization errors
        }

        private void Start()
        {
            OnCategorySelection();
        }

        public void Close()
        {
            ICommand close = new DeleteObjectCommand(gameObject, null);
            UndoRedoManager.Execute(close);
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
            UpdateModelChooser(selectedPartController);
        }

        private void ModelItemSelected(object sender, EventArgs e)
        {
            EnsureAvatarConfigController();
            AvatarPartConfigurationController selectedPartController = GetSelectedController();
            selectedPartController.ModelIndex = modelSelector.SelectedIndex;
            selectedPartController.ApplyConfiguration();

            PlayerPropertyUtilities.SetProperty(
                avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].Name + AvatarAppearanceSynchronizer.modelKeySuffix,
                (byte)selectedPartController.ModelIndex);
            PlayerPropertyUtilities.SetProperty(
                avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].Name + AvatarAppearanceSynchronizer.materialKeySuffix,
                (byte)selectedPartController.MaterialIndex);
            PlayerPropertyUtilities.SetProperty(
                avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].Name + AvatarAppearanceSynchronizer.colorKeySuffix,
                (byte)selectedPartController.ColorIndex);


            UpdateMaterialChooser(selectedPartController);
        }

        private void MaterialItemSelected(object sender, EventArgs e)
        {
            EnsureAvatarConfigController();
            AvatarPartConfigurationController selectedPartController = GetSelectedController();
            selectedPartController.MaterialIndex = materialSelector.SelectedIndex;
            selectedPartController.ApplyConfiguration();

            PlayerPropertyUtilities.SetProperty(
                avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].Name + "Material",
                (byte)materialSelector.SelectedIndex);

            UpdateColorChooser(selectedPartController);
        }

        private void ColorItemSelected(object sender, EventArgs e)
        {
            EnsureAvatarConfigController();
            AvatarPartConfigurationController selectedPartController = GetSelectedController();
            selectedPartController.ColorIndex = colorSelector.SelectedIndex;
            selectedPartController.ApplyConfiguration();

            PlayerPropertyUtilities.SetProperty(
                avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].Name + "Color",
                (byte)colorSelector.SelectedIndex);
        }

        private void UpdateModelChooser(AvatarPartConfigurationController selectedPartController)
        {
            modelSelector.Items = selectedPartController.AvatarParts;
            modelSelector.SelectedIndex = selectedPartController.ModelIndex;
            modelSelector.Page = modelSelector.SelectedIndex / modelSelector.ItemFrameCount;

            UpdateMaterialChooser(selectedPartController);
        }

        private void UpdateMaterialChooser(AvatarPartConfigurationController selectedPartController)
        {
            materialSelector.Items = selectedPartController.AvatarPartMaterials;
            materialSelector.SelectedIndex = selectedPartController.MaterialIndex;
            materialSelector.Page = materialSelector.SelectedIndex / materialSelector.ItemFrameCount;

            UpdateColorChooser(selectedPartController);
        }

        private void UpdateColorChooser(AvatarPartConfigurationController selectedPartController)
        {
            colorSelector.Items = selectedPartController.AvatarPartColorsAsItems;
            colorSelector.SelectedIndex = selectedPartController.ColorIndex;
            colorSelector.Page = colorSelector.SelectedIndex / colorSelector.ItemFrameCount;
        }

        private AvatarPartConfigurationController GetSelectedController()
        {
            return avatarConfigurationController.AvatarPartControllers[categoryToggles.CurrentIndex].ConfigurationController;
        }
    }
}