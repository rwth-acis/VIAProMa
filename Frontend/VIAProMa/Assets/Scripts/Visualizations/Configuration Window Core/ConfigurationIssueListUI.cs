using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    public class ConfigurationIssueListUI : MonoBehaviour, IUiFragment
    {
        [SerializeField] private Interactable selectionButton;
        [SerializeField] private GameObject selectionActiveMessage;

        private bool uiEnabled = true;
        private Visualization visualization;

        public bool UIEnabled
        {
            get => uiEnabled;
            set
            {
                uiEnabled = value;
                selectionButton.Enabled = uiEnabled;
            }
        }

        private void Awake()
        {
            if (selectionButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionButton));
            }

            if (selectionActiveMessage == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectionActiveMessage));
            }
            else
            {
                selectionActiveMessage.SetActive(false);
            }
        }

        public void Setup(Visualization visualization)
        {
            this.visualization = visualization;
        }

        public void OpenIssueList()
        {
            var issues = visualization.ContentProvider.Issues;
            var x = new IssueListViewItem();
            foreach (var issue in issues)
            {
                Debug.Log(issue.Name);
            }

            UIEnabled = false;
            selectionActiveMessage.SetActive(true);
        }

        public void CloseIssueList()
        {
            UIEnabled = true;
            selectionActiveMessage.SetActive(false);
        }
    }
}