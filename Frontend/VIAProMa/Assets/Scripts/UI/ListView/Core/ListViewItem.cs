using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewItem<DataType> : MonoBehaviour
    where DataType : IListViewItemData
{
    protected IListViewController controller;
    protected int index;

    protected DataDisplay<DataType> display;

    private void Awake()
    {
        if (display == null)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        display = GetComponent<DataDisplay<DataType>>();
        if (display == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(DataDisplay<DataType>), gameObject);
        }
    }

    public virtual void Setup(DataType content, int index, IListViewController controller)
    {
        if (display == null)
        {
            Initialize();
        }
        display.Setup(content);
        this.index = index;
        this.controller = controller;
        UpdateView();
    }

    protected virtual void UpdateView()
    {
        display.UpdateView();
    }

    public void ItemSelected()
    {
        controller.OnItemSelected(index);
    }
}

public class ListViewItem : ListViewItem<ListViewItemInspectorData>
{
}

[System.Serializable]
public class ListViewItemInspectorData : IListViewItemData
{
}