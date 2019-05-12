using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputField : MonoBehaviour
{
    [SerializeField] private TextMeshPro contentField;

    [SerializeField] private string text = "Content";

    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            ApplyTextToDisplay();
        }
    }

    private void Awake()
    {
        if (contentField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(contentField));
        }
    }

    private void ApplyTextToDisplay()
    {
        if (contentField != null)
        {
            contentField.text = text;
        }
    }

    private void OnValidate()
    {
        ApplyTextToDisplay();
    }
}
