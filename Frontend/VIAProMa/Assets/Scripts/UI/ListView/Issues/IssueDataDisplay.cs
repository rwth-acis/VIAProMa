using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IssueDataDisplay : DataDisplay<Issue>
{
    [SerializeField] private TextMeshPro titleField;
    [SerializeField] private TextMeshPro descriptionField;
    [SerializeField] private UserDataDisplay creatorDisplay;
    [SerializeField] private IssueStatusDisplay statusDisplay;

    private void Awake()
    {
        if (titleField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(titleField));
        }
        if(descriptionField == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(descriptionField));
        }
        if(creatorDisplay == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(creatorDisplay));
        }
        if (statusDisplay == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusDisplay));
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        titleField.text = content.Name;
        descriptionField.text = content.Description;
        creatorDisplay.Setup(content.Creator);
        statusDisplay.Setup(content);
    }
}
