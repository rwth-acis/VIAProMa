using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Highlights all selected renderers if the object is focused
/// </summary>
public class FocusHighlight : MonoBehaviour, IMixedRealityFocusHandler
{
    [Tooltip("Renderers which should be affected by the focus highlight")]
    [SerializeField] private Renderer[] targetRenderers;

    /// <summary>
    /// The color which the renderers should have if they are focused
    /// </summary>
    public Color focusColor;

    private Color[] defaultColors;

    /// <summary>
    /// Checks the component setup and initializes the default color array with the current colors of the renderer's materials
    /// </summary>
    private void Awake()
    {
        if (targetRenderers.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(targetRenderers));
        }

        defaultColors = new Color[targetRenderers.Length];
        for (int i=0;i<targetRenderers.Length;i++)
        {
            if (targetRenderers[i] == null)
            {
                SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(targetRenderers), i);
            }
            else
            {
                defaultColors[i] = targetRenderers[i].material.color;
            }
        }
    }

    /// <summary>
    /// Called if the object is focused
    /// Sets the color of the targetRenderers to the focusColor
    /// </summary>
    /// <param name="eventData">event data of the focus</param>
    public void OnFocusEnter(FocusEventData eventData)
    {
        SetColorForAll(focusColor);
    }

    /// <summary>
    /// Called if the object loses focus
    /// Sets teh color of the targetRenderers back to their default colors
    /// </summary>
    /// <param name="eventData"></param>
    public void OnFocusExit(FocusEventData eventData)
    {
        for (int i=0;i<targetRenderers.Length;i++)
        {
            targetRenderers[i].material.color = defaultColors[i];
        }
    }

    /// <summary>
    /// Sets the color of the material of all renderers to newColor
    /// </summary>
    /// <param name="newColor">The color to which hte renderers should be set</param>
    private void SetColorForAll(Color newColor)
    {
        foreach(Renderer targetRenderer in targetRenderers)
        {
            targetRenderer.material.color = newColor;
        }
    }
}
