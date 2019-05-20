using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewItemAdapter<DataType> : MonoBehaviour
    where DataType : IListViewItemData
{
    protected DataType content;

    public virtual void Setup(DataType content)
    {
        this.content = content;
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