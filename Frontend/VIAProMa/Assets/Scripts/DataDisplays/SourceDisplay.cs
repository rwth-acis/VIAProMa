using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SourceDisplay : DataDisplay<Issue>
{
    [SerializeField] private TextMeshPro sourceLabel;
    [SerializeField] private Renderer backgroundRenderer;

    private void Awake()
    {
        if (sourceLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(sourceLabel));
        }
        if (backgroundRenderer == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(backgroundRenderer));
        }
    }

    public override void UpdateView()
    {
        base.UpdateView();
        if (content != null)
        {
            switch (content.Source)
            {
                case DataSource.GITHUB:
                    SetSourceView("GitHub", Color.white, new Color(36f / 255f, 41f / 255f, 46f / 255f)); // black
                    break;
                case DataSource.REQUIREMENTS_BAZAAR:
                    SetSourceView("Requirements Bazaar", Color.white, new Color(68f / 255f, 117f / 255f, 0f)); // green
                    break;
            }
        }
        else
        {
            SetSourceView("Error", Color.white, new Color(134f / 255f, 4f / 255f, 127f / 255f)); // purple
        }
    }

    private void SetSourceView(string text, Color textColor, Color backgroundColor)
    {
        sourceLabel.text = text;
        sourceLabel.color = textColor;
        backgroundRenderer.material.color = backgroundColor;
    }
}
