using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationIssueSelectionUI : MonoBehaviour
{
    [SerializeField] private Interactable selectionButton;
    [SerializeField] private GameObject selectionActiveMessage;

    private bool uiEnabled = true;
    private Visualization visualization;

    public bool UIEnabled
    {
        get => uiEnabled;
        set
        {
            uiEnabled = value;
            selectionButton.Enabled = uiEnabled;
        }
    }

    private void Awake()
    {
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

    public void Setup(Visualization visualization)
    {
        this.visualization = visualization;
    }

    public void SelectIssues()
    {
        visualization.ContentProvider.SelectContent();
        UIEnabled = false;
        selectionActiveMessage.SetActive(true);
    }

    public void EndIssueSelection()
    {
        visualization.ContentProvider.EndContentSelection();
        UIEnabled = true;
        selectionActiveMessage.SetActive(false);
    }
}
