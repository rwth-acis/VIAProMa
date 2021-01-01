using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.DataDisplays
{
    /// <summary>
    /// Controls the visual presentation of the status of an issue
    /// </summary>
    public class IssueStatusDisplay : DataDisplay<Issue>
    {
        [SerializeField] private TextMeshPro statusLabel;
        [SerializeField] private Renderer statusLabelBackground;

        /// <summary>
        /// Checks the setup
        /// </summary>
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

        /// <summary>
        /// Updates the view representation
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();
            if (content != null)
            {
                if (content.Status == IssueStatus.OPEN)
                {
                    SetStatusDisplay("Open", Color.white, new Color(181f / 255f, 25f / 255f, 25f / 255)); // red
                }
                else if (content.Status == IssueStatus.IN_PROGRESS)
                {
                    SetStatusDisplay("Development", Color.black, new Color(245f / 255f, 233f / 255f, 6f / 255f)); // yellow
                }
                else if (content.Status == IssueStatus.CLOSED)
                {
                    SetStatusDisplay("Closed", Color.black, new Color(7f / 255f, 187f / 255f, 40f / 255f)); // green
                }
            }
            else // if no issue is provided, just show an error status
            {
                SetStatusDisplay("Error", Color.black, new Color(134f / 255f, 4f / 255f, 127f / 255f)); // purple
            }
        }

        /// <summary>
        /// Sets the text and color of the status display
        /// </summary>
        /// <param name="text">The text for the label of the status display</param>
        /// <param name="backgroundColor">The background color of the status display</param>
        private void SetStatusDisplay(string text, Color textColor, Color backgroundColor)
        {
            statusLabel.text = text;
            statusLabel.color = textColor;
            statusLabelBackground.material.color = backgroundColor;
        }
    }
}