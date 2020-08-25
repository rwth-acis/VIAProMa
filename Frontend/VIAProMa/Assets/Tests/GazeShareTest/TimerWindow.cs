using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerWindow : MonoBehaviour, IWindow
{
    [SerializeField] private InputField saveNameInputField;
    [SerializeField] private Interactable doneButton;
    [SerializeField] private GameObject icon;
    [SerializeField] private GameObject caption;
    [SerializeField] private Sprite play;
    [SerializeField] private Sprite stop;
    public static bool timerOn { get; private set; }

    public bool WindowEnabled { get; set; }

    public bool WindowOpen
    {
        get => gameObject.activeSelf;
    }

    public event EventHandler WindowClosed;

    private void Awake()
    {
        
    }

    private void Start()
    {
        timerOn = false;
        saveNameInputField.Text = SaveLoadManager.Instance.SaveName;
        icon.GetComponent<SpriteRenderer>().sprite = play;
    }

    private void OnSaveNameChanged(object sender, EventArgs e)
    {
        bool validInput = !string.IsNullOrWhiteSpace(saveNameInputField.Text);
        doneButton.Enabled = validInput;
    }

    public void ToggleTimer()
    {
        if (timerOn == true)
        {
            //stop the timer
            timerOn = false;
            saveNameInputField.Text = "00:00";
            //doneButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
            caption.GetComponent<TextMeshPro>().SetText("Start");
            icon.GetComponent<SpriteRenderer>().sprite = play;
        }
        else
        {
            //start the timer
            timerOn = true;
            saveNameInputField.Text = "Timer runs...";
            //doneButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            caption.GetComponent<TextMeshPro>().SetText("Stop");
            icon.GetComponent<SpriteRenderer>().sprite = stop;
        }
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
