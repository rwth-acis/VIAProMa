using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for measuring the size of text
/// It uses an invisible textMesh in the scene, loads the text in there with the given settigns and measures its size
/// </summary>
public class TextSize : Singleton<TextSize>
{
    /// <summary>
    /// The textMesh which is used to try out the text configurations
    /// </summary>
    private TextMesh textMesh;

    /// <summary>
    /// The renderer of the text mesh; it is required in order to get the correct sizes
    /// </summary>
    private Renderer textMeshRenderer;

    /// <summary>
    /// Instantiates the script, i.e. it creates the textMesh and gets its components
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        textMesh = GetComponent<TextMesh>();

        // the textMesh should not be visible in the scene
        textMesh.gameObject.SetActive(false);

        if (textMesh == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(TextMesh), gameObject);
            return;
        }
        textMesh.transform.position = Vector3.zero;
        textMesh.transform.rotation = Quaternion.identity;

        textMeshRenderer = textMesh.gameObject.GetComponent<Renderer>();
    }

    /// <summary>
    /// Measures the size of text in world units
    /// </summary>
    /// <param name="text">The text to measure</param>
    /// <param name="fontSize">The font size of the text</param>
    /// <returns>The size of the text in the given font size in world units</returns>
    public Vector2 GetTextSize(string text, int fontSize)
    {
        textMesh.fontSize = fontSize;
        textMesh.text = text;
        return textMeshRenderer.bounds.size;
    }
}
