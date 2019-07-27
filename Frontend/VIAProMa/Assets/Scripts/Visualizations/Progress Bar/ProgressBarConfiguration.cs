using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarConfiguration : ConfigurationWindow
{
    [SerializeField] private ConfigurationIssueSelectionUI issueSelectionUI;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            issueSelectionUI.UIEnabled = value;
        }
    }

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

    public override void Open()
    {
        base.Open();
        transform.position = appBarSpawner.SpawnedInstance.transform.position - 0.05f * appBarSpawner.SpawnedInstance.transform.forward;
        transform.rotation = appBarSpawner.SpawnedInstance.transform.rotation;
    }
}
