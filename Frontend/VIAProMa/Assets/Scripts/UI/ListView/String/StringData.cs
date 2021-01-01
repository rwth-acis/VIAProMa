using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.UI.ListView.Strings
{
    [Serializable]
    public class StringData : IListViewItemData
    {
        public string text;

        public StringData()
        {
        }

        public StringData(string text)
        {
            this.text = text;
        }
    }
}