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
            var issues = visualization.ContentProvider.Issues;
            //var issueViewList = gameObject.AddComponent(typeof(IssuesMultiListView)) as IssuesMultiListView;
            //issueViewList.Items = issues;
            foreach (var issue in issues)
            {
                Debug.Log(issue.Name);
            }

            listWindow.SetActive(true);
            UIEnabled = false;
            listWindow.SetActive(true);
        }

        public void CloseIssueList()
        {
            UIEnabled = true;
            listWindow.SetActive(false);
        }
    }
}