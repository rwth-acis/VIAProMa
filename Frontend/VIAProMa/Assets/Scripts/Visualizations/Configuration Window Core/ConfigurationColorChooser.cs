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

    private Visualization visualization;
    private InteractableToggleCollection toggleCollection;

    private bool uiEnabled = true;
    private int colorIndex;

    private Interactable selectedSquare;

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
        if (colorPreviewSquare == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorPreviewSquare));
        }
        if (colorSquareArray == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorSquareArray));
        }

        toggleCollection = colorSquareArray.GetComponent<InteractableToggleCollection>();
        if (toggleCollection == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(toggleCollection), colorSquareArray.gameObject);
        }

        toggleCollection.ToggleList = new Interactable[colorConfiguration.Colors.Count];
        for (int i=0;i<colorConfiguration.Colors.Count;i++)
        {
            GameObject instance = Instantiate(colorPreviewSquare, colorSquareArray.transform);
            ColorPreviewSquare square = instance.GetComponent<ColorPreviewSquare>();
            square.Color = colorConfiguration.Colors[i];
            Interactable interactable = instance.GetComponent<Interactable>();
            toggleCollection.ToggleList[i] = interactable;
        }
        colorSquareArray.UpdateCollection();
    }

    public void Setup(Visualization visualization)
    {
        this.visualization = visualization;
    }

    public void ChooseColor(Interactable square, Color color)
    {
        visualization.Color = color;
    }

    private void SelectSquare(Interactable newSelected, Color color)
    {
        visualization.Color = color;
    }
}
