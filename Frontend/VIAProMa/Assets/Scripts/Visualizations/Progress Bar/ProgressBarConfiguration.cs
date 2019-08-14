using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration window for progress bars
/// </summary>
public class ProgressBarConfiguration : ConfigurationWindow
{
    [Tooltip("The UI which handles the issue selection")]
    [SerializeField] private ConfigurationIssueSelectionUI issueSelectionUI;

    /// <summary>
    /// Gets or sets whether the window is enabled
    /// If the window is disabled, all controls will be disabled so that the user cannot interact with the window anymore
    /// </summary>
    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            issueSelectionUI.UIEnabled = value;
        }
    }

    /// <summary>
    /// Checks the component's setup
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (issueSelectionUI == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueSelectionUI));
        }
        else
        {
            issueSelectionUI.Setup(visualization);
        }
    }

    /// <summary>
    /// Opens the configuration window
    /// Determines the position where the window should be shown
    /// </summary>
    public override void Open()
    {
        base.Open();
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
        transform.rotation = appBarSpawner.SpawnedInstance.transform.rotation;
    }
}
