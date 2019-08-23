using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationWindow : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] protected AppBarConfigurableSpawner appBarSpawner;
    [SerializeField] protected Visualization visualization;

    [Header("UI Elements")]
    [SerializeField] protected Interactable closeButton;
    [SerializeField] protected InputField progressBarTitleField;

    private bool windowEnabled;

    public virtual bool WindowEnabled
    {
        get => windowEnabled;
        set
        {
            windowEnabled = value;
            closeButton.Enabled = windowEnabled;
            progressBarTitleField.Enabled = windowEnabled;
        }
    }

    public bool WindowOpen { get; private set; }

    public event EventHandler WindowClosed;

    protected virtual void Awake()
    {
        if (appBarSpawner == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(appBarSpawner));
        }
        if (visualization == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(visualization));
        }
        if (closeButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(closeButton));
        }
        if (progressBarTitleField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBarTitleField));
        }
        else
        {
            progressBarTitleField.Text = visualization.Title;
            progressBarTitleField.TextChanged += TitleChanged;
        }
    }

    public virtual void Close()
    {
        WindowOpen = false;
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        WindowOpen = true;
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        // do not set the position and eulerAngles since the configuration window should have a fixed position
    }

    protected virtual void TitleChanged(object sender, EventArgs e)
    {
        visualization.Title = progressBarTitleField.Text;
    }
}
