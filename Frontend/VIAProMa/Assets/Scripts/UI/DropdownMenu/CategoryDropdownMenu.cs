using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.UI.DropdownMenu.Core;
using i5.VIAProMa.UI.ListView.Categories;
using UnityEngine;

namespace i5.VIAProMa.UI.DropdownMenu
{
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
}