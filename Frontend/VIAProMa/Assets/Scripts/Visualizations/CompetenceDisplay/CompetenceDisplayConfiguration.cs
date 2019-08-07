using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurationIssueSelectionUI))]
public class CompetenceDisplayConfiguration : ConfigurationWindow
{
    [SerializeField] private InputField filterInputField;

    private ConfigurationIssueSelectionUI issueSelection;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            issueSelection.UIEnabled = value;
            filterInputField.Enabled = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (filterInputField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(filterInputField));
        }
        else
        {
            filterInputField.Text = "";
            filterInputField.TextChanged += FilterChanged;
        }

        issueSelection = GetComponent<ConfigurationIssueSelectionUI>();
        issueSelection.Setup(visualization);
        ((CompetenceDisplay)visualization).FilterWords = new string[0];
    }

    private void FilterChanged(object sender, EventArgs e)
    {
        string filterText = filterInputField.Text;
        // separate by ";"
        string[] filter = filterText.Split(';');
        // clean up whitespace characters
        for (int i=0;i<filter.Length;i++)
        {
            filter[i] = filter[i].Trim();
        }

        // apply filter
        ((CompetenceDisplay)visualization).FilterWords = filter;
        visualization.UpdateView();
    }
}
