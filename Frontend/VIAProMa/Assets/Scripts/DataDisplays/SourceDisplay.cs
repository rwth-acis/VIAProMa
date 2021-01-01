using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.DataDisplays
{
    /// <summary>
    /// Controls the view representation of the source display of an issue
    /// </summary>
    public class SourceDisplay : DataDisplay<Issue>
    {
        [SerializeField] private TextMeshPro sourceLabel;
        [SerializeField] private Renderer backgroundRenderer;

        /// <summary>
        /// Checks the setup of the component
        /// </summary>
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

        /// <summary>
        /// Updates the view based on the data source of the issue
        /// </summary>
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

        /// <summary>
        /// Sets the text and colors for the source view
        /// </summary>
        /// <param name="text">The text for the source label</param>
        /// <param name="textColor">The text color of the source label</param>
        /// <param name="backgroundColor">The color of the background color</param>
        private void SetSourceView(string text, Color textColor, Color backgroundColor)
        {
            sourceLabel.text = text;
            sourceLabel.color = textColor;
            backgroundRenderer.material.color = backgroundColor;
        }
    }
}