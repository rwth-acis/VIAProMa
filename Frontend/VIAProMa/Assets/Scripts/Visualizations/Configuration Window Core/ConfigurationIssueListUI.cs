﻿using System;
using System.Collections.Generic;
using i5.VIAProMa.DataModel.API;
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
        [SerializeField] private GameObject emptyMessage;

        private bool uiEnabled = true;
        private Visualization visualization;
        private int currentPage = 0;
        private IssuesMultiListView issueViewList;
        private int numberOfIssuesPerPage;
        private const int NumberOfListViews = 3;

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
                issueViewList = listWindow.GetComponent(typeof(IssuesMultiListView)) as IssuesMultiListView;
                if (issueViewList)
                {
                    numberOfIssuesPerPage = NumberOfListViews * issueViewList.numberOfItemsPerListView;
                }
            }
        }

        public void Setup(Visualization pVisualization)
        {
            visualization = pVisualization;
        }

        public void OpenIssueList()
        {
            listWindow.SetActive(true);
            UIEnabled = false;
            visualization.ContentProvider.ContentChanged += OnContentChanged;
            ReloadIssueList();
        }

        private void OnContentChanged(object sender, EventArgs e)
        {
            ReloadIssueList();
        }

        private void ReloadIssueList()
        {
            var issues = visualization.ContentProvider.Issues;
            var issuesCount = issues.Count;
            if (issueViewList)
            {
                if (issuesCount == 0)
                {
                    ChangeEmptyMessageVisibility(true);
                }
                else
                {
                    ChangeEmptyMessageVisibility(false);
                    if (issuesCount <= currentPage * numberOfIssuesPerPage + 1)
                    {
                        // The page number is too high. There are not enough issues assigned to this visualization.
                        // Some issues were probably removed. We have to lower the page number
                        currentPage = issues.Count / numberOfIssuesPerPage;
                    }

                    var issuesForThisPage = new List<Issue>();
                    var issuesStartingPoint = currentPage * numberOfIssuesPerPage;
                    for (var i = issuesStartingPoint;
                         i < issuesStartingPoint + numberOfIssuesPerPage && issuesCount > i;
                         i++)
                    {
                        issuesForThisPage.Add(issues[i]);
                    }

                    issueViewList.Items = issuesForThisPage;
                }
            }
            else
            {
                LogMissingField(nameof(listWindow));
            }
        }

        private void ChangeEmptyMessageVisibility(bool visible)
        {
            if (emptyMessage)
            {
                emptyMessage.SetActive(visible);
            }
            else
            {
                LogMissingField(nameof(emptyMessage));
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void LogMissingField(string referenceName)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, referenceName);
        }

        public void PageDown()
        {
            currentPage++;
            ReloadIssueList();
        }

        public void PageUp()
        {
            if (currentPage == 0) return;
            currentPage--;
            ReloadIssueList();
        }

        public void CloseIssueList()
        {
            visualization.ContentProvider.ContentChanged -= OnContentChanged;
            UIEnabled = true;
            listWindow.SetActive(false);
        }
    }
}