using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisualStyleLoader : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private VisualCustomizationConfiguration configuration;
    [SerializeField] private VisualStyle[] styles;

    private void OnEnable()
    {
        ReloadStyle();
        VisualCustomizationManager.updateStyles += ReloadStyle;
    }

    private void OnDisable()
    {
        VisualCustomizationManager.updateStyles -= ReloadStyle;
    }

    public void LoadStyle(string styleKey, string variantKey)
    {
        foreach (var style in styles)
        {
            if (style.key == styleKey)
            {
                style.gameObject.SetActive(true);
                foreach (var variant in style.variants)
                {
                    if (variant.key == variantKey)
                    {
                        variant.ApplyVariant();
                    }
                }
            }
            else
            {
                style.gameObject.SetActive(false);
            }
        }
    }
    
    private void ReloadStyle()
    {
        var loadedStyle = VisualCustomizationManager.FindCurrentStyle(key);
        LoadStyle(loadedStyle.style, loadedStyle.variation);
    }
    
    public void RegisterStyles()
    {
        if (configuration)
        {
            configuration.AddEntry(GenerateStyleEntry());
        }
    }

    private VisualCustomizationConfiguration.StyleConfiguration GenerateStyleEntry()
    {
        var newEntry = new VisualCustomizationConfiguration.StyleConfiguration();
        newEntry.key = key;

        foreach (var style in styles)
        {
            var newStyle = new VisualCustomizationConfiguration.StyleEntry();
            newStyle.key = style.key;
            foreach (var variant in style.variants)
            {
                var newVariant = new VisualCustomizationConfiguration.StyleVariantEntry();
                newVariant.key = variant.key;

                newStyle.styleVariantEntryEntries ??= new List<VisualCustomizationConfiguration.StyleVariantEntry>();
                newStyle.styleVariantEntryEntries.Add(newVariant);
            }

            newEntry.styleEntries ??= new List<VisualCustomizationConfiguration.StyleEntry>();
            newEntry.styleEntries.Add(newStyle);
        }

        return newEntry;
    }
    
    public void FindStyles()
    {
        styles = GetComponentsInChildren<VisualStyle>(true);
    }
}

[CustomEditor(typeof(VisualStyleLoader))]
public class VisualStyleLoaderEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        VisualStyleLoader myScript = (VisualStyleLoader)target;
        if(GUILayout.Button("Find Styles"))
        {
            myScript.FindStyles();
        }
        if(GUILayout.Button("Register Styles"))
        {
            myScript.RegisterStyles();
        }
    }
}