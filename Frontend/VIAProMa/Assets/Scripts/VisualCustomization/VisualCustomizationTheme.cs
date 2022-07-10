using System;
using System.Collections;
using System.Collections.Generic;
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

    [Serializable]
    public class StyleSelection
    {
        public string key;
        public string style;
        public string variation;
    }
}
