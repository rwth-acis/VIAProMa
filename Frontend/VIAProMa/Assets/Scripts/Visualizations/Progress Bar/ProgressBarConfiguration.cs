using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarConfiguration : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] private AppBarConfigurableSpawner appBarSpawner;
    [SerializeField] private ProgressBar progressBar;

    [Header("UI Elements")]
    [SerializeField] private Interactable closeButton;
    [SerializeField] private InputField progressBarTitleField;
    [SerializeField] private Interactable selectionButton;
    [SerializeField] private GameObject selectionActiveMessage;

    public event EventHandler WindowClosed;

    private bool windowEnabled;

    public bool WindowEnabled
    {
        get => windowEnabled;
        set
        {
            windowEnabled = value;
            closeButton.Enabled = windowEnabled;
            progressBarTitleField.Enabled = windowEnabled;
            selectionButton.Enabled = windowEnabled;
        }
    }

    private void Awake()
    {
        if (appBarSpawner == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(appBarSpawner));
        }
        if (progressBar == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
        }

        if (closeButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(closeButton));
        }
        if (progressBarTitleField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBarTitleField));
        }
        if (selectionButton == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionButton));
        }
        if (selectionActiveMessage == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionActiveMessage));
        }
        else
        {
            selectionActiveMessage.SetActive(false);
        }
        progressBarTitleField.Text = progressBar.Title;
        progressBarTitleField.TextChanged += ProgressBarTitleChanged;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
        transform.rotation = appBarSpawner.SpawnedInstance.transform.rotation;
    }

    public void SelectIssues()
    {
        progressBar.ContentProvider.SelectContent();
        WindowEnabled = false;
        selectionActiveMessage.SetActive(true);
    }

    public void EndIssueSelection()
    {
        progressBar.ContentProvider.EndContentSelection();
        WindowEnabled = true;
        selectionActiveMessage.SetActive(false);
    }

    private void ProgressBarTitleChanged(object sender, EventArgs e)
    {
        progressBar.Title = progressBarTitleField.Text;
    }

}
