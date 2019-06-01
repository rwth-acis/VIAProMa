using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryDropdownMenu : DropdownMenu<Category, CategoryListViewItem>
{
    [SerializeField] private CategoryDataDisplay categorySelectedItemDisplay;
    [SerializeField] private CategoryListView categoryListViewController;

    protected override void Awake()
    {
        // workarounds because the generic types are not recognized by Unity's inspector
        selectedItemDisplay = categorySelectedItemDisplay;
        itemController = categoryListViewController;
        base.Awake();
    }
}
