using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//Scripted Object used to configure possible styles and default themes
[CreateAssetMenu(fileName = "Configuration", menuName = "Scriptable Objects/VisualCustomization/Configuration", order = 1)]
public class VisualCustomizationConfiguration : ScriptableObject
{
    //Contains all possible styles
    public List<StyleConfiguration> styleEntries;
    //The non user generated styles
    public List<VisualCustomizationTheme> themes;

    //Updated by the Button in the StyleLoader, adds an entry for a style to  all possible entries
    public void AddEntry(StyleConfiguration newConfiguration)
    {
        //remove all entries with the same key
        for (var index = 0; index < styleEntries.Count; index++)
        {
            var entry = styleEntries[index];
            if (entry.key == newConfiguration.key)
            {
                styleEntries.RemoveAt(index);
                index--;
            }
        }

        styleEntries.Add(newConfiguration);
        EditorUtility.SetDirty(this);
        Debug.Log("Added Entry to Configuration", this);
    }
    
    //Creates a new theme with all styles set to the default value
    public void AddTheme()
    {
        var defaultStyleSelection = new List<VisualCustomizationTheme.StyleSelection>();
        
        foreach (var entry in styleEntries)
        {
            var selection = new VisualCustomizationTheme.StyleSelection();
            selection.key = entry.key;
            if (entry.styleEntries.Count > 0)
            {
                selection.style = entry.styleEntries[0].key;
                if (entry.styleEntries[0].styleVariantEntryEntries.Count > 0)
                {
                    selection.variation = entry.styleEntries[0].styleVariantEntryEntries[0].key;
                }
            }
            
            defaultStyleSelection.Add(selection);
        }

        themes.Add(new VisualCustomizationTheme(defaultStyleSelection));
        EditorUtility.SetDirty(this);
    }
    
    //Updates all existing themes to include newly added Styles
    public void UpdateThemes()
    {
        foreach (var visualCustomizationTheme in themes)
        {
            foreach (var styleConfiguration in styleEntries)
            {
                if (!visualCustomizationTheme.ContainsKey(styleConfiguration.key))
                {
                    var newStyleSelection = new VisualCustomizationTheme.StyleSelection
                    {
                        key = styleConfiguration.key,
                        style = styleConfiguration.styleEntries[0].key,
                        variation = styleConfiguration.styleEntries[0].styleVariantEntryEntries[0].key
                    };
                    
                    visualCustomizationTheme.styleSelections.Add(newStyleSelection);
                }
            }
        }
        
        EditorUtility.SetDirty(this);
    }

    //Returns the first theme in the List. It is treated as the default theme
    public VisualCustomizationTheme GetDefaultTheme()
    {
        if (themes == null || themes.Count < 0)
        {
            Debug.LogError("Please create at least one theme", this);
            return null;
        }
        return themes[0];
    }
    
    [Serializable]
    public class StyleConfiguration
    {
        public string key;
        public List<StyleEntry> styleEntries;
    }

    [Serializable]
    public class StyleEntry
    {
        public string key;
        public List<StyleVariantEntry> styleVariantEntryEntries;
    }
    
    [Serializable]
    public class StyleVariantEntry
    {
        public string key;
    }
}

[CustomEditor(typeof(VisualCustomizationConfiguration))]
public class VisualCustomizationConfigurationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (VisualCustomizationConfiguration)target;
        if (GUILayout.Button("Create new Theme"))
        {
            myScript.AddTheme();
        }
        if (GUILayout.Button("Update Themes"))
        {
            myScript.UpdateThemes();
        }
    }
}