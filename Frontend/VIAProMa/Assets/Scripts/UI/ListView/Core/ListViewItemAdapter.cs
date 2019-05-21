using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewItemAdapter<DataType> : MonoBehaviour
    where DataType : IListViewItemData
{
    protected DataType content;
    protected IListViewController controller;
    protected int index;

    public virtual void Setup(DataType content, int index, IListViewController controller)
    {
        this.content = content;
        this.index = index;
        this.controller = controller;
        UpdateView();
    }

    protected virtual void UpdateView()
    {
    }
}

public class ListViewItemAdapter : ListViewItemAdapter<ListViewItemInspectorData>
{
}

[System.Serializable]
public class ListViewItemInspectorData : IListViewItemData
{
}