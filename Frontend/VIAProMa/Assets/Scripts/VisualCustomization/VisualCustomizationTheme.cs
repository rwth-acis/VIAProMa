using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class VisualCustomizationTheme
{
    public string name;
    public List<StyleSelection> styleSelections;

    public VisualCustomizationTheme(List<StyleSelection> styleSelections)
    {
        this.styleSelections = styleSelections;
    }

    //Checks if the theme has a styleSelection for the given key
    public bool ContainsKey(string key)
    {
        return styleSelections.Any(styleSelection => styleSelection.key == key);
    }
    
    [Serializable]
    public class StyleSelection
    {
        public string key;
        public string style;
        public string variation;
    }
}
