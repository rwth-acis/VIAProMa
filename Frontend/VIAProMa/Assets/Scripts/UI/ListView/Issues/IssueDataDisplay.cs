using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.ListView.Issues
{
    public class IssueDataDisplay : DataDisplay<Issue>
    {
        [SerializeField] private TextMeshPro titleField;
        [SerializeField] private TextMeshPro descriptionField;
        [SerializeField] private UserDataDisplay creatorDisplay;
        [SerializeField] private IssueStatusDisplay statusDisplay;
        [SerializeField] private SourceDisplay sourceDisplay;

        private void Awake()
        {
            if (titleField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(titleField));
            }
            if (descriptionField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(descriptionField));
            }
            if (creatorDisplay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(creatorDisplay));
            }
            if (statusDisplay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(statusDisplay));
            }
            if (sourceDisplay == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(sourceDisplay));
            }
        }

        public override void UpdateView()
        {
            base.UpdateView();
            if (content != null)
            {
                titleField.text = content.Name;
                descriptionField.text = content.Description;
                creatorDisplay.Setup(content.Creator);
                statusDisplay.Setup(content);
                sourceDisplay.Setup(content);
            }
            else
            {
                titleField.text = "Error while loading";
                descriptionField.text = "";
                creatorDisplay.Setup(null);
                statusDisplay.Setup(null);
                sourceDisplay.Setup(null);
            }
        }
    }
}