using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarConfiguration : MonoBehaviour, IWindow
{
    [Header("References")]
    [SerializeField] private AppBarSpawner appBarSpawner;
    [SerializeField] private ProgressBar progressBar;

    [Header("UI Elements")]
    [SerializeField] private InputField progressBarTitleField;
    [SerializeField] private Interactable selectionButton;

    public event EventHandler WindowClosed;

    private bool windowEnabled;

    public bool WindowEnabled
    {
        get => windowEnabled;
        set
        {
            windowEnabled = value;
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
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
    }

    public void SelectIssues()
    {
        progressBar.ContentProvider.SelectContent();
    }

    public void EndIssueSelection()
    {
        progressBar.ContentProvider.EndContentSelection();
    }
}
