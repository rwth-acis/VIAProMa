﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputField : MonoBehaviour
{
    [SerializeField] private TextMeshPro contentField;

    [SerializeField] private string text = "Content";

    public event EventHandler TextChanged;

    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            ApplyTextToDisplay();
            TextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public TextMeshPro ContentField { get => contentField; }

    private void Awake()
    {
        if (contentField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(contentField));
        }
    }

    public void OnClick()
    {
        Keyboard.Instance.Open(transform.position - transform.forward * 0.05f, transform.eulerAngles, Text);
        Keyboard.Instance.InputFinished += OnKeyboardInputFinished;
    }

    private void OnKeyboardInputFinished(object sender, InputFinishedEventArgs e)
    {
        Keyboard.Instance.InputFinished -= OnKeyboardInputFinished;
        if (!e.Aborted)
        {
            Text = e.Text;
        }
    }

    private void OnKeyboardTextChanged(object sender, EventArgs e)
    {
        Text = Keyboard.Instance.Text;
    }

    private void ApplyTextToDisplay()
    {
        if (contentField != null)
        {
            contentField.text = text;
        }
    }

    public void ResetText()
    {
        Text = "";
    }

    private void OnValidate()
    {
        ApplyTextToDisplay();
    }
}
