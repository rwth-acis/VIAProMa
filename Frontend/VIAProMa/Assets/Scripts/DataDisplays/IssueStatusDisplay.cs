using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IssueStatusDisplay : DataDisplay<Issue>
{
    [SerializeField] private TextMeshPro statusLabel;
    [SerializeField] private Renderer statusLabelBackground;

    private void Awake()
    {
        if (statusLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusLabel));
        }
        if (statusLabelBackground == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusLabelBackground));
        }
    }

    public override void Setup(Issue issue)
    {
        content = issue;
        UpdateView();
    }

    public override void UpdateView()
    {
        base.UpdateView();
        if (content != null)
        {
            if (content.Status == IssueStatus.OPEN)
            {
                SetStatusDisplay("Open", new Color(7f / 255f, 187f / 255f, 40f / 255f)); // green
            }
            else if (content.Status == IssueStatus.IN_PROGRESS)
            {
                SetStatusDisplay("Development", new Color(245f / 255f, 233f / 255f, 6f / 255f)); // yellow
            }
            else if (content.Status == IssueStatus.CLOSED)
            {
                SetStatusDisplay("Closed", new Color(181f / 255f, 25f / 255f, 25f / 255)); // red
            }
        }
        else
        {
            SetStatusDisplay("Error", new Color(134f / 255f, 4f / 255f, 127f / 255f)); // purple
        }
    }

    private void SetStatusDisplay(string text, Color backgroundColor)
    {
        statusLabel.text = text;
        statusLabelBackground.material.color = backgroundColor;
    }
}
