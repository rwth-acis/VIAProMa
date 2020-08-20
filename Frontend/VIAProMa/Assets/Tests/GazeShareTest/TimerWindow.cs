using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerWindow : MonoBehaviour, IWindow
{
    [SerializeField] private InputField saveNameInputField;
    [SerializeField] private Interactable doneButton;

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
            //start the timer
            saveNameInputField.Text = "Timer runs";
        }
        else
        {
            saveNameInputField.Text = "00:00";
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

    public async void SetSaveName()
    {
        SaveLoadManager.Instance.SaveName = saveNameInputField.Text;

        await SaveLoadManager.Instance.SaveScene();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }
}
