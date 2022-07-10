using System;
using System.Collections.Generic;
using System.Linq;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.UI.InputFields;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    /// <summary>
    /// UI controller and actions for customizing Visual Customization Themes
    /// </summary>
    public class VisualThemeEditorMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private VisualCustomizationMenu mainMenu;
        [SerializeField] private InputField nameInput;
        [SerializeField] private Interactable deleteButton;
        [SerializeField] private Interactable doneButton;
        [SerializeField] private GameObject warningLabel;
        [SerializeField] private TextMeshPro objectKeyLabel;
        [SerializeField] private TextMeshPro styleKeyLabel;
        [SerializeField] private TextMeshPro variantKeyLabel;

        [SerializeField] private VisualCustomizationKeySelector objectKeySelector;
        [SerializeField] private VisualCustomizationKeySelector styleKeySelector;
        [SerializeField] private VisualCustomizationKeySelector variantKeySelector;
        
        public bool WindowEnabled { get; set; }

        private VisualCustomizationTheme editing;
        private VisualCustomizationTheme originalTheme;
        private List<VisualCustomizationConfiguration.StyleConfiguration> configurations;

        private int keyIndex = 0;
        private int styleIndex = 1;
        private int variantIndex = 1;
        
        public bool WindowOpen => gameObject.activeSelf;

        public event EventHandler WindowClosed;

        private void OnEnable()
        {
            nameInput.TextChanged += UpdateDone;
        }

        private void OnDisable()
        {
            nameInput.TextChanged -= UpdateDone;
        }

        private void UpdateDone(object sender, EventArgs e)
        {
            UpdateDone();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            mainMenu.Activate();
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            if (VisualCustomizationManager.IsInitialized)
            {
                configurations = VisualCustomizationManager.GetPossibleConfigurations();
            }
            
            gameObject.SetActive(true);
            mainMenu.Deactivate();
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        public void LoadTheme(VisualCustomizationTheme theme)
        {
            originalTheme = theme;
            editing = new VisualCustomizationTheme(theme.styleSelections)
            {
                name = theme.name
            };
            nameInput.Text = theme.name;
            deleteButton.IsEnabled = !VisualCustomizationManager.IsDefaultTheme(theme.name) && originalTheme.name != "";
            SetKey(theme.styleSelections[0].key);
            SetStyle(theme.styleSelections[0].style);
            SetVariant(theme.styleSelections[0].variation);
            UpdateLabels();
            UpdateDone();
        }

        private void UpdateDone()
        {
            if (VisualCustomizationManager.IsDefaultTheme(nameInput.Text))
            {
                doneButton.IsEnabled = false;
                warningLabel.SetActive(true);
            }
            else if (nameInput.Text.Trim() != "")
            {
                doneButton.IsEnabled = true;
                warningLabel.SetActive(false);
            }
            else
            {
                doneButton.IsEnabled = false;
                warningLabel.SetActive(false);
            }
        }

        private void SetKey(string key)
        {
            for (var index = 0; index < configurations.Count; index++)
            {
                var styleConfiguration = configurations[index];
                if (styleConfiguration.key == key)
                {
                    keyIndex = index;
                    return;
                }
            }
        }
        
        private void SetStyle(string style)
        {
            for (var index = 0; index < configurations[keyIndex].styleEntries.Count; index++)
            {
                var styleEntry = configurations[keyIndex].styleEntries[index];
                if (styleEntry.key == style)
                {
                    styleIndex = index;
                    return;
                }
            }
        }
        
        private void SetVariant(string variant)
        {
            for (var index = 0;
                 index < configurations[keyIndex].styleEntries[styleIndex].styleVariantEntryEntries.Count;
                 index++)
            {
                var variantEntry =
                    configurations[keyIndex].styleEntries[styleIndex].styleVariantEntryEntries[index];
                if (variantEntry.key == variant)
                {
                    variantIndex = index;
                    return;
                }
            }
        }
        
        private void UpdateLabels()
        {
            objectKeyLabel.text = configurations[keyIndex].key;
            styleKeyLabel.text = configurations[keyIndex].styleEntries[styleIndex].key;
            variantKeyLabel.text = configurations[keyIndex].styleEntries[styleIndex]
                .styleVariantEntryEntries[variantIndex].key;

            //Deactivate Buttons if there is only one option
            if (configurations.Count < 2)
            {
                objectKeySelector.Deactivate();
            }
            else
            {
                objectKeySelector.Activate();
            }
            
            if (configurations[keyIndex].styleEntries.Count < 2)
            {
                styleKeySelector.Deactivate();
            }
            else
            {
                styleKeySelector.Activate();
            }
            
            if (configurations[keyIndex].styleEntries[styleIndex].styleVariantEntryEntries.Count < 2)
            {
                variantKeySelector.Deactivate();
            }
            else
            {
                variantKeySelector.Activate();
            }
        }

        public void NextKey()
        {
            keyIndex = mod(keyIndex + 1,configurations.Count);
            SetStyle(editing.styleSelections[keyIndex].style);
            SetVariant(editing.styleSelections[keyIndex].variation);
            UpdateLabels();
        }
        
        public void PrevKey()
        {
            keyIndex = mod(keyIndex - 1, configurations.Count);
            SetStyle(editing.styleSelections[keyIndex].style);
            SetVariant(editing.styleSelections[keyIndex].variation);
            UpdateLabels();
        }
        
        public void NextStyle()
        {
            styleIndex = mod(styleIndex + 1,configurations[keyIndex].styleEntries.Count);
            editing.styleSelections[keyIndex].style = configurations[keyIndex].styleEntries[styleIndex].key;
            SetVariant(editing.styleSelections[keyIndex].variation);
            UpdateLabels();
        }
        
        public void PrevStyle()
        {
            styleIndex = mod(styleIndex - 1, configurations[keyIndex].styleEntries.Count);
            editing.styleSelections[keyIndex].style = configurations[keyIndex].styleEntries[styleIndex].key;
            SetVariant(editing.styleSelections[keyIndex].variation);
            UpdateLabels();
        }
        
        public void NextVariant()
        {
            variantIndex = mod(variantIndex + 1,configurations[keyIndex].styleEntries[styleIndex].styleVariantEntryEntries.Count);
            editing.styleSelections[keyIndex].variation = configurations[keyIndex].styleEntries[styleIndex]
                .styleVariantEntryEntries[variantIndex].key;
            UpdateLabels();
        }
        
        public void PrevVariant()
        {
            variantIndex = mod(variantIndex - 1, configurations[keyIndex].styleEntries[styleIndex].styleVariantEntryEntries.Count);
            editing.styleSelections[keyIndex].variation = configurations[keyIndex].styleEntries[styleIndex]
                .styleVariantEntryEntries[variantIndex].key;
            UpdateLabels();
        }
        
        //custom modulo to correctly deal with negative numbers
        private int mod(int x, int m) {
            return (x%m + m)%m;
        }

        public void SaveTheme()
        {
            editing.name = nameInput.Text;
            VisualCustomizationManager.AddCustomTheme(editing);
            VisualCustomizationManager.SwitchTheme(editing);
            mainMenu.ReloadThemes();
            Close();
        }
        
        public void DeleteTheme()
        {
            VisualCustomizationManager.RemoveCustomTheme(originalTheme.name);
            mainMenu.ReloadThemes();
            Close();
        }
    }
}