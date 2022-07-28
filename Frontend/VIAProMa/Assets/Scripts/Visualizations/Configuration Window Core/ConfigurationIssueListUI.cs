using System;
using System.Collections.Generic;
using System.Linq;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.ColorConfigWindow
{
    public class ConfigurationIssueListUI : MonoBehaviour, IUiFragment
    {
        [Tooltip("This button should open the \"List Visualization Content\" window.")] [SerializeField]
        private Interactable selectionButton;

        [Tooltip("This actual  \"List Visualization Content\" window.")] [SerializeField]
        private GameObject listWindow;

        [Tooltip("These display the actual issues.")] [SerializeField]
        private List<IssueListView> issueViewLists;

        [Tooltip("How many issues should be displayed per IssueListView from left to right.")] [SerializeField]
        private int horizontalIssueViewListSize;

        [Tooltip("The empty message shown when a  \"List Visualization Content\" window is opened but no issues are " +
                 "added to the visualization.")]
        [SerializeField]
        private GameObject emptyMessage;

        [Tooltip("The up button. When pushed go back to the previous page.")] [SerializeField]
        private GameObject upButton;

        [Tooltip("The down button. When pushed go to the next page.")] [SerializeField]
        private GameObject downButton;

        private bool uiEnabled = true;
        private Visualization visualization;
        private int currentPage;
        private int numberOfIssuesPerPage;
        private Interactable upButtonInteractable;
        private Interactable downButtonInteractable;

        public bool UIEnabled
        {
            get => uiEnabled;
            set
            {
                uiEnabled = value;
                selectionButton.IsEnabled = uiEnabled;
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
                numberOfIssuesPerPage = issueViewLists.Count * horizontalIssueViewListSize;

                if (upButton)
                {
                    upButtonInteractable = upButton.GetComponent<Interactable>();
                }
                else
                {
                    LogMissingField(nameof(upButton));
                }

                if (downButton)
                {
                    downButtonInteractable = downButton.GetComponent<Interactable>();
                }
                else
                {
                    LogMissingField(nameof(downButton));
                }
            }
        }

        /// <summary>
        /// Initializes this object. This has to be called from your ConfigurationWindow class.
        /// </summary>
        /// <param name="pVisualization">The visualization of which the issues should be displayed</param>
        public void Setup(Visualization pVisualization)
        {
            visualization = pVisualization;
        }

        /// <summary>
        /// Initializes the issues list window and opens the "List Visualization Content" window on the current page defined by <see cref="ConfigurationIssueListUI.currentPage"/>.
        /// </summary>
        public void OpenIssueList()
        {
            listWindow.SetActive(true);
            UIEnabled = false;
            visualization.ContentProvider.ContentChanged += OnContentChanged;
            ReloadIssueList();
        }

        /// <summary>
        /// The content of the visualization has changed. Reload the issue list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnContentChanged(object sender, EventArgs e)
        {
            ReloadIssueList();
        }

        private void ReloadIssueList()
        {
            var issues = visualization.ContentProvider.Issues;
            var issuesCount = issues.Count;
            if (issueViewLists.Any())
            {
                if (issuesCount == 0)
                {
                    // No task cards/issues assigned. Show empty message.
                    ChangeEmptyMessageVisibility(true);
                }
                else
                {
                    // There are task cards/issues assigned. Don't show empty message.
                    ChangeEmptyMessageVisibility(false);
                    var maxPage = (issuesCount - 1) / (numberOfIssuesPerPage);
                    if (currentPage > maxPage)
                    {
                        // The page number is too high. There are not enough issues assigned to this visualization.
                        // Some issues/task cards were probably removed. We have to lower the page number
                        currentPage = maxPage;
                    }

                    // Fill <see cref="ConfigurationIssueListUI.issueViewLists"/> with issues of this page
                    var issuesStartingPoint = currentPage * numberOfIssuesPerPage;
                    for (var j = 0; j < issueViewLists.Count; j++)
                    {
                        var start = issuesStartingPoint + j * horizontalIssueViewListSize;
                        var issuesForThisIssueView = new List<Issue>();
                        for (var i = start;
                             i < start + horizontalIssueViewListSize && issuesCount > i;
                             i++)
                        {
                            issuesForThisIssueView.Add(issues[i]);
                        }

                        issueViewLists[j].Items = issuesForThisIssueView;
                    }

                    if (downButton)
                    {
                        // Enable down button if there are more pages
                        downButtonInteractable.IsEnabled = currentPage != maxPage;
                    }
                    else
                    {
                        LogMissingField(nameof(upButton));
                    }

                    if (upButton)
                    {
                        // Enable up button if there are previous pages
                        upButtonInteractable.IsEnabled = currentPage != 0;
                    }
                    else
                    {
                        LogMissingField(nameof(upButton));
                    }
                }
            }
            else
            {
                LogMissingField(nameof(issueViewLists));
            }
        }

        private void ChangeEmptyMessageVisibility(bool visible)
        {
            if (emptyMessage)
            {
                emptyMessage.SetActive(visible);
                if (upButton)
                {
                    upButton.SetActive(!visible);
                }
                else
                {
                    LogMissingField(nameof(upButton));
                }

                if (downButton)
                {
                    downButton.SetActive(!visible);
                }
                else
                {
                    LogMissingField(nameof(downButton));
                }
            }
            else
            {
                LogMissingField(nameof(emptyMessage));
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// This method calls <see cref="SpecialDebugMessages.LogMissingReferenceError"/>.
        ///
        /// This method is used instead of calling <see cref="SpecialDebugMessages.LogMissingReferenceError"/> directly,
        /// as the performance analyzer thinks that <see cref="SpecialDebugMessages.LogMissingReferenceError"/> is expensive,
        /// which it is, but which does not matter, as this method should only be rarely called, as it indicates the
        /// window was setup wrongly in the editor, as there should be no missing fields.
        /// </summary>
        /// <param name="referenceName">The name of the reference which is missing</param>
        private void LogMissingField(string referenceName)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, referenceName);
        }

        /// <summary>
        /// The page down button was clicked. Add 1 to the page count and then reload the the issue list to display the
        /// new page.
        /// </summary>
        public void PageDown()
        {
            currentPage++;
            ReloadIssueList();
        }

        /// <summary>
        /// The page up button was clicked. Subtract 1 from the page count and then reload the the issue list to display the
        /// new page.
        /// </summary>
        public void PageUp()
        {
            currentPage--;
            ReloadIssueList();
        }

        /// <summary>
        /// The close button was clicked. Close the "List Visualization Content" window. And reenable the "List Visualization Content" button.
        /// </summary>
        public void CloseIssueList()
        {
            visualization.ContentProvider.ContentChanged -= OnContentChanged;
            UIEnabled = true;
            listWindow.SetActive(false);
        }
    }
}