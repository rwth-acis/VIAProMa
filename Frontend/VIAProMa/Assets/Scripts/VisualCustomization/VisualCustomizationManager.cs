using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer.Chat;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class VisualCustomizationManager : Singleton<VisualCustomizationManager>
{
    public static UnityAction updateStyles;

    [SerializeField] private VisualCustomizationConfiguration configuration;
    [SerializeField] private List<StyleSelection> currentStyleSelections;

    public void SetStylesDefault()
    {
        currentStyleSelections = new List<StyleSelection>();
        
        foreach (var entry in configuration.styleEntries)
        {
            var selection = new StyleSelection();
            selection.key = entry.key;
            if (entry.styleEntries.Count > 0)
            {
                selection.style = entry.styleEntries[0].key;
                if (entry.styleEntries[0].styleVariantEntryEntries.Count > 0)
                {
                    selection.variation = entry.styleEntries[0].styleVariantEntryEntries[0].key;
                }
            }
            
            currentStyleSelections.Add(selection);
        }
        
        EditorUtility.SetDirty(this);
    }

    public static StyleSelection FindCurrentStyle(string key)
    {
        foreach (var styleSelection in Instance.currentStyleSelections)
        {
            if (styleSelection.key == key)
            {
                return styleSelection;
            }
        }

        return new StyleSelection();
    }

    public static void SwitchStyle(string key, string style, string variation)
    {
        var styleSelection = new StyleSelection
        {
            key = key,
            style = style,
            variation = variation
        };
        
        SwitchStyle(styleSelection);
    }
    
    public static void SwitchStyle(StyleSelection newStyle)
    {
        for (var index = 0; index < Instance.currentStyleSelections.Count; index++)
        {
            var selection = Instance.currentStyleSelections[index];
            if (selection.key == newStyle.key)
            {
                selection.style = newStyle.style;
                selection.variation = newStyle.variation;
                
                updateStyles?.Invoke();
                return;
            }
        }
    }

    [ContextMenu("switch style Test")]
    public void SwitchStyleTest()
    {
         SwitchStyle("Shelf", "Alternative", "Red");
    }
    [ContextMenu("switch style Test 2")]
    public void SwitchStyleTest2()
    {
        SwitchStyle("Shelf", "Default", "Default");
    }
    [ContextMenu("switch style Test 3")]
    public void SwitchStyleTest3()
    {
        SwitchStyle("Shelf", "Default", "Red");
    }
    
    
    [Serializable]
    public class StyleSelection
    {
        public string key;
        public string style;
        public string variation;
    }
}

[CustomEditor(typeof(VisualCustomizationManager))]
public class VisualCustomizationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (VisualCustomizationManager)target;
        if (GUILayout.Button("Reset to default Styles"))
        {
            myScript.SetStylesDefault();
        }
    }
}

