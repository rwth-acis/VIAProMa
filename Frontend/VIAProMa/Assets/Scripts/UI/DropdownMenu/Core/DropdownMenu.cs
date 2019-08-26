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

    public event EventHandler<EventArgs> ItemSelected;

    public List<DataType> Items
    {
        get { return itemController?.Items; }
        set
        {
            itemController.Items = value;
            // by standard select the first element
            if (Items != null && Items.Count > 0)
            {
                selectedItemDisplay.Setup(Items[0]);
            }
        }
    }

    public int SelectedItemIndex
    {
        get { return itemController.SelectedItemIndex; }
        set
        {
            if (Items != null && Items.Count > value)
            {
                itemController.SelectedItemIndex = value;
                selectedItemDisplay.Setup(Items[value]);
            }
        }
    }

    public DataType SelectedItem
    {
        get
        {
            return Items[SelectedItemIndex];
        }
    }

    public bool DropdownListShown
    {
        get
        {
            return itemController.gameObject.activeSelf;
        }
        set
        {
            itemController.gameObject.SetActive(value);
        }
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
        itemController.ItemSelected += OnItemSelected;
        // by standard select the first element
        if (Items != null && Items.Count > 0)
        {
            selectedItemDisplay.Setup(Items[0]);
        }
        // hide the dropdownlist
        DropdownListShown = false;
    }

    private void OnItemSelected(object sender, ListViewItemSelectedArgs e)
    {
        selectedItemDisplay.Setup(Items[e.SelectedItemIndex]);
        DropdownListShown = false;
        ItemSelected?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleListVisibility()
    {
        DropdownListShown = !DropdownListShown;
    }

    private void OnDestroy()
    {
        if (itemController != null)
        {
            itemController.ItemSelected -= OnItemSelected;
        }
    }
}

public class DropdownMenu : DropdownMenu<ListViewItemInspectorData, ListViewItem>
{
}
