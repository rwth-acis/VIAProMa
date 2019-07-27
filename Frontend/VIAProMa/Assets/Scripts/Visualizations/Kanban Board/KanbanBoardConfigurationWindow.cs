using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardConfigurationWindow : MonoBehaviour, IWindow
{
    [SerializeField] 

    public bool WindowEnabled { get; set; }

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
}
