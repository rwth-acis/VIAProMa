using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarConfiguration : ConfigurationWindow
{
    [SerializeField] private Interactable selectionButton;
    [SerializeField] private GameObject selectionActiveMessage;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            progressBarTitleField.Enabled = value;
            selectionButton.Enabled = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

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
    }

    public override void Open()
    {
        base.Open();
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
        transform.rotation = appBarSpawner.SpawnedInstance.transform.rotation;
    }

    public void SelectIssues()
    {
        visualization.ContentProvider.SelectContent();
        WindowEnabled = false;
        selectionActiveMessage.SetActive(true);
    }

    public void EndIssueSelection()
    {
        visualization.ContentProvider.EndContentSelection();
        WindowEnabled = true;
        selectionActiveMessage.SetActive(false);
    }
}
