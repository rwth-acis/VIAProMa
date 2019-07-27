using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationColorChooser : MonoBehaviour, IUiFragment
{
    [SerializeField] private GameObject colorPreviewSquare;
    [SerializeField] private GridObjectCollection colorSquareArray;

    public ColorConfiguration colorConfiguration;

    private bool uiEnabled = true;
    private int colorIndex;

    public bool UIEnabled
    {
        get => uiEnabled;
        set
        {
            uiEnabled = value;
        }
    }

    public Color SelectedColor
    {
        get => colorConfiguration.Colors[colorIndex];
    }

    public int ColorIndex
    {
        get => colorIndex;
        set
        {
            colorIndex = Mathf.Clamp(value, 0, colorConfiguration.Colors.Count);
        }
    }

    private void Awake()
    {
        for (int i=0;i<colorConfiguration.Colors.Count;i++)
        {
            GameObject instance = Instantiate(colorPreviewSquare, colorSquareArray.transform);
            ColorPreviewSquare square = instance.GetComponent<ColorPreviewSquare>();
            square.Color = colorConfiguration.Colors[i];
            square.ColorIndex = i;
        }
        colorSquareArray.UpdateCollection();
    }

    public void Setup(Visualization visualization)
    {
    }
}
