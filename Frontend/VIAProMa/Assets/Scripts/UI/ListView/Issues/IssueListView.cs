using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Core;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI.ListView.Issues
{
    public class IssueListView : ListViewController<Issue, IssueListViewItem>
    {
        [SerializeField] private GameObject createIssueButtonPrefab;

        protected override void RemoveInstances()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            if(this.name == "ShelfBoard (4)")
            {
                GameObject createIssueButton = Instantiate(createIssueButtonPrefab);
                createIssueButton.transform.parent = transform;
            }
        }
    }
}