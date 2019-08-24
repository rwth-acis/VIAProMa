using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main controller of the list view
/// Takes items of an DataType and transforms them to GameObject instances.
/// The ItemType is a class which can apply the DataType to the GameObject's representation, e.g. fill the text meshes
/// </summary>
/// <typeparam name="DataType">The DataType of the list</typeparam>
/// <typeparam name="ItemType">The class which converts a DataType to a GameObject representation</typeparam>
public class ListViewController<DataType, ItemType> : MonoBehaviour, IListViewController
    where DataType : IListViewItemData
    where ItemType : ListViewItem<DataType>
{
    [SerializeField] protected GameObject itemPrefab;

    [SerializeField] protected List<DataType> items = new List<DataType>();

    public event EventHandler<ListViewItemSelectedArgs> ItemSelected;

    protected List<ItemType> instances;

    public List<DataType> Items
    {
        get => items;
        set
        {
            items = value;
            RemoveInstances();
            CreateInstances();
            SelectedItemIndex = 0;
        }
    }

    public GameObject ItemPrefab
    {
        get => itemPrefab;
    }

    public int SelectedItemIndex { get; private set; }

    public DataType SeletedItem { get { return items[SelectedItemIndex]; } }

    private void Awake()
    {
        instances = new List<ItemType>();
        if (itemPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(itemPrefab));
        }
        else
        {
            if (itemPrefab.GetComponent<ItemType>() == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ItemType), itemPrefab);
            }
        }
        if (Items == null)
        {
            Items = new List<DataType>();
        }
    }

    protected virtual void CreateInstances()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            ItemType instanceAdapter = Instantiate(itemPrefab, transform).GetComponent<ItemType>();
            if (instanceAdapter == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ItemType), itemPrefab);
            }
            else
            {
                instanceAdapter.Setup(items[i], i, this);
            }
        }
    }

    protected virtual void RemoveInstances()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void OnItemSelected(int index)
    {
        SelectedItemIndex = index;
        EventHandler<ListViewItemSelectedArgs> handler = ItemSelected;
        if (handler != null)
        {
            ListViewItemSelectedArgs args = new ListViewItemSelectedArgs();
            args.SelectedItemIndex = index;
            handler(this, args);
        }
    }

    public void Clear()
    {
        Items = new List<DataType>();
    }
}

public class ListViewController : ListViewController<ListViewItemInspectorData, ListViewItem>
{
}

public class ListViewItemSelectedArgs : EventArgs
{
    public int SelectedItemIndex { get; set; }
}
