using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the menu which allows a user to select existing rooms (or navigate to the menu where a new room can be created)
/// </summary>
public class virtualEnvironmentsMenu : MonoBehaviour, IWindow
{

    [SerializeField] private Interactable pageUpButton;
    [SerializeField] private Interactable pageDownButton;


    /// <summary>
    /// The number of room entries which are shown on one page
    /// </summary>
    public int entriesPerPage = 2;

    private int page = 0;
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
            pageUpButton.Enabled = value;
            pageDownButton.Enabled = value;
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

    /// <summary>
    /// Initializes the component, makes sure that it is set up correctly
    /// </summary>
    private void Awake()
    {
        if (pageUpButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageUpButton));
        }
        if (pageDownButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(pageDownButton));
        }

        Close();
    }

   
    /// <summary>
    /// Called if the user pushes the page up button
    /// Swiches to the previous page
    /// </summary>
    public void PageUp()
    {
        page = Mathf.Max(0, page - 1);
        SetPageButtonStates();
    }

    /// <summary>
    /// Called if the user pages the page down button
    /// Switches to the next page
    /// </summary>
    public void PageDown()
    {
        page = Mathf.Min(page + 1, 5);
        SetPageButtonStates();
    }


    /// <summary>
    /// Adapts the button states of the page up and page down buttons
    /// If the first page is shown, the up button is disabled and if the last page is shown, the down button is disabled
    /// </summary>
    private void SetPageButtonStates()
    {
        if (page == 0) // first page
        {
            pageUpButton.Enabled = false;
        }
        else
        {
            pageUpButton.Enabled = true;
        }

        if (page == 5) // last page
        {
            pageDownButton.Enabled = false;
        }
        else
        {
            pageDownButton.Enabled = true;
        }
    }

    /// <summary>
    /// Opens the window
    /// </summary>
    public void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    public void Close()
    {
        WindowOpen = false;
        WindowClosed?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
