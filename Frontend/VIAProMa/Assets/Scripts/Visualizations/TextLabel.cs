using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextLabel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshPro[] textLabels;
    [SerializeField] private Transform background;

    [Header("Values")]
    [SerializeField] private string text;
    [SerializeField] private float width = 1f;
    [SerializeField] private float height = 0.1f;
    [SerializeField] private float padding = 0.005f;

    private void Awake()
    {
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
        if (background == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(background));
        }

        UpdateVisuals();
    }

    public float Width
    {
        get => width;
        set
        {
            width = value;
            UpdateVisuals();
        }
    }

    public float Height
    {
        get => height;
        set
        {
            height = value;
            UpdateVisuals();
        }
    }

    public float Padding
    {
        get => padding;
        set
        {
            padding = value;
            UpdateVisuals();
        }
    }

    public string Text
    {
        get => text;
        set
        {
            text = value;
            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        gameObject.SetActive(!string.IsNullOrEmpty(text));

        for (int i = 0; i < textLabels.Length; i++)
        {
            textLabels[i].rectTransform.sizeDelta = new Vector2(width, height);
            //background.localScale = new Vector3(
            //    width,
            //    height,
            //    background.localScale.z);
            textLabels[i].text = text;
            textLabels[i].ForceMeshUpdate();
        }

        background.localScale = new Vector3(
            textLabels[0].textBounds.size.x + padding * 2f,
            textLabels[0].textBounds.size.y + padding * 2f,
            background.localScale.z
            );
    }
}
