using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Configuration", menuName = "Scriptable Objects/VisualCustomization/Configuration", order = 1)]
public class VisualCustomizationConfiguration : ScriptableObject
{
    public List<StyleConfiguration> styleEntries;

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

