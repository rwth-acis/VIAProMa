using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.UI.MultiListView.Core;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    public class ConfigurationIssueListUI : MonoBehaviour, IUiFragment
    {
        [SerializeField] private Interactable selectionButton;
        [SerializeField] private GameObject listWindow;

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

            if (listWindow == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(listWindow));
            }
            else
            {
                listWindow.SetActive(false);
            }
        }

        public void Setup(Visualization visualization)
        {
            this.visualization = visualization;
        }

        public void OpenIssueList()
        {
            listWindow.SetActive(true);
            UIEnabled = false;
            var issues = visualization.ContentProvider.Issues;
            var issueViewList = listWindow.GetComponent(typeof(IssuesMultiListView)) as IssuesMultiListView;
            if (issueViewList != null)
            {
                issueViewList.Items = issues;
            }
            else
            {
                Debug.Log("No List Window set. Please add one");
            }
        }

        public void CloseIssueList()
        {
            UIEnabled = true;
            listWindow.SetActive(false);
        }
    }
}