﻿using System.Collections;
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
            for (int listViewIndex = 0; listViewIndex < listViews.Length; listViewIndex++)
            {
                List<DataType> subItems = new List<DataType>();
                for (int itemIndex = listViewIndex * numberOfItemsPerListView; itemIndex < items.Count; itemIndex++)
                {
                    subItems.Add(items[itemIndex]);
                }
                listViews[listViewIndex].Items = subItems;
            }
        }
    }
}

public class MultiListView : MultiListView<ListViewItemInspectorData, ListViewItem>
{
}


