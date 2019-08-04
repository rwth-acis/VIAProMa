using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurationIssueSelectionUI))]
public class CompetenceDisplayConfiguration : ConfigurationWindow
{
    private ConfigurationIssueSelectionUI issueSelection;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            issueSelection.UIEnabled = value;
        }
    }

    protected override void Awake()
    {
        issueSelection = GetComponent<ConfigurationIssueSelectionUI>();
        issueSelection.Setup(visualization);
    }
}
