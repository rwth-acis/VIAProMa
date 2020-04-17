using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI controller and actions for the login menu
/// </summary>
public class TestMenu : MonoBehaviour, IWindow
{

    public bool WindowEnabled { get; set; }

    public bool WindowOpen => gameObject.activeSelf;

    public event EventHandler WindowClosed;


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