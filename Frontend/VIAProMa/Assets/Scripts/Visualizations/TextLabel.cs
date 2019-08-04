using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls text label GameObjects
/// They consist of a background and a number of textLabels with the same content
/// </summary>
[ExecuteInEditMode]
public class TextLabel : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The text labels which should display the content of the text label")]
    [SerializeField] private TextMeshPro[] textLabels;

    [Tooltip("The background of the text label")]
    [SerializeField] private Transform background;

    [Header("Values")]
    [Tooltip("The text content of the text label")]
    [SerializeField] private string text;

    [Tooltip("Maximum width of the text label; if the text is shorted, the text label may appear smaller")]
    [SerializeField] private float maxWidth = 1f;

    [Tooltip("Maximum height of the text label; if the text is smaller, the text label may appear smaller")]
    [SerializeField] private float maxHeight = 0.1f;

    [Tooltip("Padding of the background to all sides")]
    [SerializeField] private float padding = 0.005f;

    /// <summary>
    /// Checks the setup of the text label and calls UpdateVisuals() for the first time
    /// </summary>
    private void Awake()
    {
        if (!Application.isEditor)
        {
            // check the text label array
            if (textLabels.Length == 0)
            {
                SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(textLabels));
            }

            for (int i = 0; i < textLabels.Length; i++)
            {
                if (textLabels[i] == null)
                {
                    SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(textLabels), i);
                }
            }
            // check the other components
            if (background == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(background));
            }
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Maximum width of the text label
    /// If the text is shorted, the text label may appear smaller
    /// </summary>
    public float MaxWidth
    {
        get => maxWidth;
        set
        {
            maxWidth = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Maximum height of the text label
    /// If the text is smaller, the text label may appear smaller
    /// </summary>
    public float MaxHeight
    {
        get => maxHeight;
        set
        {
            maxHeight = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Padding of the background to all sides
    /// </summary>
    public float Padding
    {
        get => padding;
        set
        {
            padding = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// The text content of the text label
    /// </summary>
    public string Text
    {
        get => text;
        set
        {
            text = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Updates the visuals of the text label based on the set properties
    /// like Text, Width and Height
    /// </summary>
    private void UpdateVisuals()
    {
        // only show the text label if there is text to show
        gameObject.SetActive(!string.IsNullOrEmpty(text));

        // set up all text components
        for (int i = 0; i < textLabels.Length; i++)
        {
            textLabels[i].rectTransform.sizeDelta = new Vector2(maxWidth, maxHeight);
            textLabels[i].text = text;
            textLabels[i].ForceMeshUpdate();
        }

        if (textLabels.Length > 0)
        {
            // scale the background to fit the acutal text size
            // assuming that all labels show the same in the same size
            background.localScale = new Vector3(
                textLabels[0].textBounds.size.x + padding * 2f,
                textLabels[0].textBounds.size.y + padding * 2f,
                background.localScale.z
                );
        }
    }

    /// <summary>
    /// Called in the Editor if the serialized and public variables are changed
    /// Updates the visual appearance so that the developer can see how the text label will look during runtime
    /// </summary>
    private void OnValidate()
    {
        UpdateVisuals();
    }
}
