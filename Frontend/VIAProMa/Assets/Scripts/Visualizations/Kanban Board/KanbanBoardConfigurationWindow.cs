using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurationIssueSelectionUI))]
public class KanbanBoardConfigurationWindow : ConfigurationWindow
{
    [SerializeField] private ConfigurationColorChooser colorChooser;

    private ConfigurationIssueSelectionUI issueSelection;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            issueSelection.UIEnabled = value;
            colorChooser.UIEnabled = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        colorChooser.Setup(visualization);
        issueSelection = GetComponent<ConfigurationIssueSelectionUI>();
        issueSelection.Setup(visualization);
    }
}
