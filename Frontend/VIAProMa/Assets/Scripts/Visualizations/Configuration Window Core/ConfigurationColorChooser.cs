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
    private Color selectedColor;

    public bool UIEnabled
    {
        get => uiEnabled;
        set
        {
            uiEnabled = value;
            for (int i=0;i<toggleCollection.ToggleList.Length;i++)
            {
                toggleCollection.ToggleList[i].Enabled = value;
            }
        }
    }

    public Color SelectedColor
    {
        get => selectedColor;
        set
        {
            selectedColor = value;
            visualization.Color = selectedColor;
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
            square.ColorChooser = this;
            Interactable interactable = instance.GetComponent<Interactable>();
            toggleCollection.ToggleList[i] = interactable;
        }
        toggleCollection.enabled = true;
        colorSquareArray.UpdateCollection();
    }

    public void Setup(Visualization visualization)
    {
        this.visualization = visualization;
    }
}
