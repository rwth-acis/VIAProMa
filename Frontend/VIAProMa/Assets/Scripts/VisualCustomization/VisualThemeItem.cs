using System;
using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class VisualThemeItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private SpriteRenderer selectedSprite;
    [SerializeField] private Interactable editButton;
    
    private string themeName;

    private void OnEnable()
    {
        VisualCustomizationManager.updateStyles += UpdateSelection;
    }
    
    private void OnDisable()
    {
        VisualCustomizationManager.updateStyles -= UpdateSelection;
    }

    public void Setup(string themeName, bool editable)
    {
        this.themeName = themeName;
        label.text = themeName;
        selectedSprite.enabled = VisualCustomizationManager.CurrentTheme().name == themeName;
        editButton.IsEnabled = editable;
    }

    public void SelectTheme()
    {
        VisualCustomizationManager.SwitchTheme(themeName);
    }

    private void UpdateSelection()
    {
        selectedSprite.enabled = VisualCustomizationManager.CurrentTheme().name == themeName;
    }
}
