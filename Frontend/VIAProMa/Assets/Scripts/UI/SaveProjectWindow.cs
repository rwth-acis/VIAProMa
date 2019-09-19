using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveProjectWindow : MonoBehaviour, IWindow
{
    [SerializeField] private InputField saveNameInputField;
    [SerializeField] private Interactable doneButton;
    [SerializeField] private GameObject confirmMessage;

    public bool WindowEnabled { get; set; }

    public bool WindowOpen
    {
        get => gameObject.activeSelf;
    }

    public event EventHandler WindowClosed;

    private void Awake()
    {
        if (doneButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(doneButton));
        }
        if (saveNameInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(saveNameInputField));
        }
        else
        {
            saveNameInputField.TextChanged += OnSaveNameChanged;
        }
        if (confirmMessage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(confirmMessage));
        }
        else
        {
            confirmMessage.SetActive(false);
        }
    }

    private void Start()
    {
        saveNameInputField.Text = SaveLoadManager.Instance.SaveName;
    }

    private void OnSaveNameChanged(object sender, EventArgs e)
    {
        bool validInput = !string.IsNullOrWhiteSpace(saveNameInputField.Text);
        doneButton.Enabled = validInput;
    }

    public void SetSaveName()
    {
        SaveLoadManager.Instance.SaveName = saveNameInputField.Text;
        confirmMessage.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        confirmMessage.SetActive(false);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }
}
