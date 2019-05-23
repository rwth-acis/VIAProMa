using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropdownMenu : MonoBehaviour
{
    private ListViewController itemController;

    private void Awake()
    {
        itemController = GetComponent<ListViewController>();
        if (itemController == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(ListViewController), gameObject);
            return;
        }
        itemController.ItemSelected += ItemSelected;
    }

    private void ItemSelected(object sender, ListViewItemSelectedArgs e)
    {
    }

    private void OnDestroy()
    {
        if (itemController != null)
        {
            itemController.ItemSelected -= ItemSelected;
        }
    }
}
