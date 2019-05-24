using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu<DataType, ItemType> : MonoBehaviour
    where DataType : IListViewItemData
    where ItemType : ListViewItem<DataType>
{
    protected DataDisplay<DataType> selectedItemDisplay;

    protected ListViewController<DataType, ItemType> itemController;

    public List<DataType> Items
    {
        get { return itemController?.Items; }
    }

    protected virtual void Awake()
    {
        if (selectedItemDisplay == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(selectedItemDisplay));
        }
        if (itemController == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(itemController));
        }
        itemController.ItemSelected += ItemSelected;
    }

    private void ItemSelected(object sender, ListViewItemSelectedArgs e)
    {
        selectedItemDisplay.Setup(Items[e.SelectedItem]);
    }

    private void OnDestroy()
    {
        if (itemController != null)
        {
            itemController.ItemSelected -= ItemSelected;
        }
    }
}

public class DropdownMenu : DropdownMenu<ListViewItemInspectorData, ListViewItem>
{
}
