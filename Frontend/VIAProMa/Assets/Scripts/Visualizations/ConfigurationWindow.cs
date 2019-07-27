using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationWindow : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] private AppBarConfigurableSpawner appBarSpawner;
    [SerializeField] private Visualization visualization;

    [Header("UI Elements")]
    [SerializeField] private Interactable closeButton;
    [SerializeField] private InputField progressBarTitleField;

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
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    protected virtual void TitleChanged(object sender, EventArgs e)
    {
        visualization.Title = progressBarTitleField.Text;
    }
}
