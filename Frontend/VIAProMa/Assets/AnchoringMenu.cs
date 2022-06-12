using System;
using UnityEngine;
using i5.VIAProMa.UI;

public class AnchoringMenu : MonoBehaviour, IWindow
{
    private bool windowEnabled = true;

    /// <summary>
    /// States whether the window is enabled
    /// If set to false, the window will remain visible but all interactable controls are disabled
    /// </summary>
    /// <value></value>
    public bool WindowEnabled
    {
        get
        {
            return windowEnabled;
        }
        set
        {
            windowEnabled = value;
        }
    }

    public bool WindowOpen
    {
        get; private set;
    }

    /// <summary>
    /// Event which is invoked if the window is closed
    /// </summary>
    public event EventHandler WindowClosed;

    public void Close()
    {
        gameObject.SetActive(false);
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
