using i5.ViaProMa.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurationIssueSelectionUI))]
public class CompetenceDisplayConfiguration : ConfigurationWindow
{
    [Header("UI Elements")]
    [SerializeField] private InputField filterInputField;

    private ConfigurationIssueSelectionUI issueSelection;

    private bool externalConfiguration = false;

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

    private void OnEnable()
    {
        ((CompetenceDisplay)visualization).FilterChanged += FilterExternallyChanged;
    }

    private void OnDisable()
    {
        ((CompetenceDisplay)visualization).FilterChanged -= FilterExternallyChanged;
    }

    private void FilterChanged(object sender, EventArgs e)
    {
        if (externalConfiguration)
        {
            return;
        }

        string filterText = filterInputField.Text;
        // separate by ";"
        string[] filter = filterText.Split(';');
        // clean up whitespace characters and make everything lower case
        for (int i = 0; i < filter.Length; i++)
        {
            filter[i] = filter[i].Trim();
            filter[i] = filter[i].ToLowerInvariant();
        }

        // apply filter
        ((CompetenceDisplay)visualization).FilterWords = filter;
        visualization.UpdateView();
    }

    private void FilterExternallyChanged(object sender, EventArgs e)
    {
        externalConfiguration = true;

        string[] filterWords = ((CompetenceDisplay)visualization).FilterWords;
        string combinedFilter = "";
        for (int i=0;i<filterWords.Length;i++)
        {
            combinedFilter += filterWords[i];
            if (i < filterWords.Length -1)
            {
                combinedFilter += "; ";
            }
        }
        filterInputField.Text = combinedFilter;

        externalConfiguration = false;
    }
}
