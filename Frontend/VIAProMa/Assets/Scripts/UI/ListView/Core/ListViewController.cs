using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListViewController<DataType, ItemAdapter> : MonoBehaviour, IListViewController
    where DataType : IListViewItemData
    where ItemAdapter : ListViewItemAdapter<DataType>
{
    [SerializeField] protected GameObject itemPrefab;

    [SerializeField] protected List<DataType> items = new List<DataType>();

    public List<DataType> Items { get => items; protected set => items = value; }

    protected List<ItemAdapter> instances;

    private void Awake()
    {
        instances = new List<ItemAdapter>();
        if (itemPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(itemPrefab));
        }
        else
        {
            if (itemPrefab.GetComponent<ItemAdapter>() == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ItemAdapter), itemPrefab);
            }
        }
    }

    protected virtual void CreateInstances()
    {
        for (int i=0;i<Items.Count;i++)
        {
            ItemAdapter instanceAdapter = Instantiate(itemPrefab, transform).GetComponent<ItemAdapter>();
            if (instanceAdapter == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ItemAdapter), itemPrefab);
            }
            else
            {
                instanceAdapter.Setup(items[i], i, this);
            }
        }
    }

    protected virtual void RemoveInstances()
    {

    }

    public void ItemSelected(int index)
    {
        Debug.Log(index);
    }
}

public class ListViewController : ListViewController<ListViewItemInspectorData, ListViewItemAdapter>
{
}
