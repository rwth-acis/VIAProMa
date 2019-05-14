using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewItem<DataType> : MonoBehaviour
    where DataType : IListViewItemData
{
    protected DataType data;

    public DataType Data
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            UpdateView();
        }
    }

    protected virtual void UpdateView()
    {
    }
}

public class ListViewItem : ListViewItem<ListViewItemInspectorData>
{
}

[System.Serializable]
public class ListViewItemInspectorData : IListViewItemData
{
}