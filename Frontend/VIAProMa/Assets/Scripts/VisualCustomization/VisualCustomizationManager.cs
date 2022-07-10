using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
using i5.VIAProMa.Multiplayer.Chat;
using i5.VIAProMa.SaveLoadSystem.Core;
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
        currentTheme = configuration.GetDefaultTheme();
        var saveData = LoadSavedThemes();
        //Updates the custom themes to make sure they are up to date
        saveData.UpdateThemes(configuration.styleEntries);
        SaveCustomThemes(saveData);
        if (saveData.selectedTheme != null)
        {
            currentTheme = saveData.selectedTheme;
        }
        updateStyles?.Invoke();
    }

    //Finds the style selected in the current theme by the key of an object
    public static VisualCustomizationTheme.StyleSelection FindCurrentStyle(string key)
    {
        foreach (var styleSelection in Instance.currentTheme.styleSelections)
        {
            if (styleSelection.key == key)
            {
                return styleSelection;
            }
        }

        return new VisualCustomizationTheme.StyleSelection();
    }

    //Switches to a different theme and updates all Styles
    public static void SwitchTheme(VisualCustomizationTheme theme)
    {
        if (theme != null)
        {
            Instance.currentTheme = theme;
            SaveCustomThemes(new ThemesSaveData(Instance.currentTheme, GetCustomThemes()));
            updateStyles?.Invoke();
        }
    }
    
    public static void SwitchTheme(string name)
    {
        var theme = Instance.configuration.themes.Find(theme => theme.name == name);
        if (theme != null)
        {
            Instance.currentTheme = theme;
            updateStyles?.Invoke();
        }
    }

    public static void AddCustomTheme(VisualCustomizationTheme toSave)
    {
        var saveData = LoadSavedThemes();
        saveData.AddTheme(toSave);
        PlayerPrefs.SetString("VisualCustomizationThemes", JsonUtility.ToJson(saveData));
    }
    
    public static void SaveCustomThemes(ThemesSaveData saveData)
    {
        PlayerPrefs.SetString("VisualCustomizationThemes", JsonUtility.ToJson(saveData));
    }
    
    public static void RemoveCustomTheme(string key)
    {
        var saveData = LoadSavedThemes();
        saveData.RemoveTheme(key);
        PlayerPrefs.SetString("VisualCustomizationThemes", JsonUtility.ToJson(saveData));
        if (Instance.currentTheme.name == key)
        {
            SwitchTheme(GetDefaultTheme());
        }
    }
    
    public static ThemesSaveData LoadSavedThemes()
    {
        ThemesSaveData saveData;
        if (PlayerPrefs.HasKey("VisualCustomizationThemes"))
        {
            saveData = JsonUtility.FromJson<ThemesSaveData>(PlayerPrefs.GetString("VisualCustomizationThemes"));
        }
        else
        {
            saveData = new ThemesSaveData(Instance.currentTheme);
        }
        return saveData;
    }

    [ContextMenu("Clear Saved Themes")]
    public void ClearAllSavedThemes()
    {
        PlayerPrefs.DeleteKey("VisualCustomizationThemes");
    }

    public static VisualCustomizationTheme GetDefaultTheme()
    {
        return Instance.configuration.GetDefaultTheme();
    }
    
    public static List<VisualCustomizationTheme> GetDefaultThemes()
    {
        return Instance.configuration.themes;
    }
    
    public static List<VisualCustomizationTheme> GetCustomThemes()
    {
        return LoadSavedThemes().customThemes;
    }

    public static VisualCustomizationTheme CurrentTheme()
    {
        return Instance.currentTheme;
    }

    public static bool IsDefaultTheme(string themeName)
    {
        foreach (var visualCustomizationTheme in GetDefaultThemes())
        {
            if (visualCustomizationTheme.name == themeName)
            {
                return true;
            }
        }

        return false;
    }

    public static List<VisualCustomizationConfiguration.StyleConfiguration> GetPossibleConfigurations()
    {
        return Instance.configuration.styleEntries;
    }
}

[CustomEditor(typeof(VisualCustomizationManager))]
public class VisualCustomizationManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var myScript = (VisualCustomizationManager)target;
        
    }
}

//This class is saved to Playerprefs and used to save the selected theme and all custom Themes between sessions
[Serializable]
public class ThemesSaveData
{
    public VisualCustomizationTheme selectedTheme;
    public List<VisualCustomizationTheme> customThemes;

    public ThemesSaveData(VisualCustomizationTheme selectedTheme, List<VisualCustomizationTheme> customThemes)
    {
        this.selectedTheme = selectedTheme;
        this.customThemes = customThemes;
    }
    
    public ThemesSaveData(VisualCustomizationTheme selectedTheme)
    {
        this.selectedTheme = selectedTheme;
        this.customThemes = new List<VisualCustomizationTheme>();
    }
    
    //Adds theme to the save data. Replaces Themes with the same name if they exists
    public void AddTheme(VisualCustomizationTheme newTheme)
    {
        customThemes.RemoveAll(theme => theme.name == newTheme.name);
        
        customThemes.Add(newTheme);
    }
    
    //removes all themes with the given name
    public void RemoveTheme(string name)
    {
        customThemes.RemoveAll(theme => theme.name == name);
    }

    public void UpdateThemes(List<VisualCustomizationConfiguration.StyleConfiguration> styleConfigurations)
    {
        foreach (var visualCustomizationTheme in customThemes)
        {
            foreach (var styleConfiguration in styleConfigurations)
            {
                if (!visualCustomizationTheme.ContainsKey(styleConfiguration.key))
                {
                    var newStyleSelection = new VisualCustomizationTheme.StyleSelection
                    {
                        key = styleConfigurations[0].key,
                        style = styleConfigurations[0].styleEntries[0].key,
                        variation = styleConfigurations[0].styleEntries[0].styleVariantEntryEntries[0].key
                    };
                    
                    visualCustomizationTheme.styleSelections.Add(newStyleSelection);
                }
            }
        }
    }
}
