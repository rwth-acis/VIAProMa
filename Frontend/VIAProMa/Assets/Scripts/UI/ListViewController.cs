using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewController<DataType, ItemType> : MonoBehaviour
    where DataType : IListViewItemData
    where ItemType : ListViewItem<DataType>
{
    [SerializeField] protected GameObject itemPrefab;

    public Vector3 offset;

    public List<DataType> Data { get; protected set; }

    private void Awake()
    {
        Data = new List<DataType>();
        if (itemPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(itemPrefab));
        }
    }

    protected virtual void CreateInstances()
    {

    }
}

public class ListViewController : ListViewController<ListViewItemInspectorData, ListViewItem>
{
}
