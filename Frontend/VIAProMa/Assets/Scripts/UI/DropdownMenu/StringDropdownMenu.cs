using i5.VIAProMa.UI.DropdownMenu.Core;
using i5.VIAProMa.UI.ListView.Strings;
using UnityEngine;

namespace i5.VIAProMa.UI.DropdownMenu
{
    public class StringDropdownMenu : DropdownMenu<StringData, StringListViewItem>
    {
        [SerializeField] private StringDataDisplay stringSelectedItemDisplay;
        [SerializeField] private StringListView stringListViewController;

        protected override void Awake()
        {
            // workarounds because the generic types are not recognized by Unity's inspector
            selectedItemDisplay = stringSelectedItemDisplay;
            itemController = stringListViewController;
            base.Awake();
        }
    }
}