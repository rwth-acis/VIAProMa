using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiListView<DataType, ItemType> : MonoBehaviour
    where DataType : IListViewItemData
    where ItemType : ListViewItem<DataType>
{
    [SerializeField] protected List<DataType> items = new List<DataType>();

    protected ListViewController<DataType, ItemType>[] listViews;

    public int numberOfItemsPerListView = 5;

    protected virtual void Awake()
    {
        if (listViews.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(listViews));
        }
    }

    public List<DataType> Items
    {
        get => items;
        set
        {
            items = value;
            for (int listViewIndex = 0; listViewIndex < listViews.Length; listViewIndex++)
            {
                List<DataType> subItems = new List<DataType>();
                int startIndex = listViewIndex * numberOfItemsPerListView;
                for (int itemIndex = startIndex; itemIndex < Mathf.Min(items.Count, startIndex + numberOfItemsPerListView); itemIndex++)
                {
                    subItems.Add(items[itemIndex]);
                }
                listViews[listViewIndex].Items = subItems;
            }
        }
    }

    public int NumberOfListViews
    {
        get
        {
            return listViews.Length;
        }
    }

    public void Clear()
    {
        for (int i=0;i<listViews.Length;i++)
        {
            listViews[i].Clear();
        }
    }
}

public class MultiListView : MultiListView<ListViewItemInspectorData, ListViewItem>
{
}


