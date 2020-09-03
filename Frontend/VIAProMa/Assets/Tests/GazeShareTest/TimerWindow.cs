using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
//using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
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
    [SerializeField] private GameObject targetPrefab;
    private GameObject target;

    //private bool firstStart;
    //private string elapsedTime;

    public bool WindowEnabled { get; set; }

    public bool WindowOpen
    {
        get => gameObject.activeSelf;
    }

    public event EventHandler WindowClosed;

    private void Awake() {}

    private void Start()
    {
        //timer = new Stopwatch();
        //time = timer.Elapsed;
        //elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", time.Minutes, time.Seconds, time.Milliseconds / 10);
        saveNameInputField.Text = GazeShareTester_Evaluation.elapsedTime;

        //saveNameInputField.Text = SaveLoadManager.Instance.SaveName;
        icon.GetComponent<SpriteRenderer>().sprite = play;

        //firstStart = true;
    }

    private void Update()
    {
        saveNameInputField.Text = GazeShareTester_Evaluation.elapsedTime;
    }

    private void OnSaveNameChanged(object sender, EventArgs e)
    {
        bool validInput = !string.IsNullOrWhiteSpace(saveNameInputField.Text);
        doneButton.Enabled = validInput;
    }

    public void ToggleTimer()
    {
        if (GazeShareTester_Evaluation.timer.IsRunning == true)
        {
            //stop the timer
            GazeShareTester_Evaluation.timer.Stop();
            GazeShareTester_Evaluation.timer.Reset();
            caption.GetComponent<TextMeshPro>().SetText("Start");
            icon.GetComponent<SpriteRenderer>().sprite = play;
        }
        else
        {
            //start the timer
            GazeShareTester_Evaluation.timer.Start();
            caption.GetComponent<TextMeshPro>().SetText("Stop");
            icon.GetComponent<SpriteRenderer>().sprite = stop;
            /*if (firstStart == true)
            {
                target = ResourceManager.Instance.NetworkInstantiate(targetPrefab, new Vector3(-1.3f, 0.3f, 2.5f), Quaternion.Euler(0, 180, 0));
                firstStart = false;
            }*/
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
