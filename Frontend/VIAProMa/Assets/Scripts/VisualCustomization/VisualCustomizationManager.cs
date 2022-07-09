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
    [SerializeField] private VisualCustomizationTheme currentTheme;

    private void Start()
    {
        updateStyles.Invoke();
    }

    

    public static VisualCustomizationTheme.StyleSelection FindCurrentStyle(string key)
    {
        if (Instance.currentTheme == null)
        {
            Instance.configuration.GetDefaultTheme();
        }
        foreach (var styleSelection in Instance.currentTheme.styleSelections)
        {
            if (styleSelection.key == key)
            {
                return styleSelection;
            }
        }

        return new VisualCustomizationTheme.StyleSelection();
    }

    public static void SwitchStyle(string key, string style, string variation)
    {
        var styleSelection = new VisualCustomizationTheme.StyleSelection
        {
            key = key,
            style = style,
            variation = variation
        };
        
        SwitchStyle(styleSelection);
    }
    
    public static void SwitchStyle(VisualCustomizationTheme.StyleSelection newStyle)
    {
        for (var index = 0; index < Instance.currentTheme.styleSelections.Count; index++)
        {
            var selection = Instance.currentTheme.styleSelections[index];
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
            //myScript.GenerateDefaultTheme();
        }
    }
}

