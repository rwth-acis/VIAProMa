using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutocompleteItem : MonoBehaviour
{
    [SerializeField] private TextMeshPro textLabel;

    private string text;
    private Keyboard keyboard;

    public string Text
    {
        get => text;
        set
        {
            text = value;
            UpdateDisplay();
        }
    }

    private void Awake()
    {
        if (textLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(textLabel));
        }
        else
        {
            Text = "";
        }
    }

    public void Setup(Keyboard keyboard)
    {
        this.keyboard = keyboard;
    }

    public void Select()
    {
        if (keyboard != null)
        {
            keyboard.Text = Text;
            keyboard.CursorPos = Text.Length;
        }
        else
        {
            Debug.LogError("User selected auto complete item but it is not yet set up");
        }
    }

    private void UpdateDisplay()
    {
        textLabel.text = text;
        gameObject.SetActive(!string.IsNullOrEmpty(text));
    }
}
