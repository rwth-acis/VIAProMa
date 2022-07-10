using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class VisualThemeItem : MonoBehaviour
{
    [SerializeField] private VisualCustomizationMenu menu;
    [SerializeField] private TextMeshPro label;
    [SerializeField] private SpriteRenderer selectedSprite;
    [SerializeField] private Interactable editButton;
    [SerializeField] private Interactable themeButton;
    
    private VisualCustomizationTheme theme;

    private void OnEnable()
    {
        VisualCustomizationManager.updateStyles += UpdateSelection;
    }
    
    private void OnDisable()
    {
        VisualCustomizationManager.updateStyles -= UpdateSelection;
    }

    public void Setup(VisualCustomizationTheme theme)
    {
        this.theme = theme;
        label.text = theme.name;
        selectedSprite.enabled = VisualCustomizationManager.CurrentTheme().name == theme.name;
    }

    public void SelectTheme()
    {
        VisualCustomizationManager.SwitchTheme(theme);
    }

    private void UpdateSelection()
    {
        if (theme != null && VisualCustomizationManager.IsInitialized && VisualCustomizationManager.CurrentTheme() != null)
        {
            selectedSprite.enabled = VisualCustomizationManager.CurrentTheme().name == theme.name;
        }
    }

    public void Activate()
    {
        editButton.IsEnabled = true;
        themeButton.IsEnabled = true;
    }
    
    public void Deactivate()
    {
        editButton.IsEnabled = false;
        themeButton.IsEnabled = false;
    }

    public void EditTheme()
    {
        menu.OpenEditor(theme);
    }
}
