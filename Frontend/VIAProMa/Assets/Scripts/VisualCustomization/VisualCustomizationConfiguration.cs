using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Configuration", menuName = "Scriptable Objects/VisualCustomization/Configuration", order = 1)]
public class VisualCustomizationConfiguration : ScriptableObject
{
    public List<StyleConfiguration> styleEntries;

    public List<VisualCustomizationTheme> themes;

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
    
    public void UpdateThemes()
    {
        //TODO

        EditorUtility.SetDirty(this);
    }

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
            //myScript.GenerateDefaultTheme();
        }
    }
}