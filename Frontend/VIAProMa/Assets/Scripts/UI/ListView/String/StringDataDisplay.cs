using i5.VIAProMa.UI.ListView.Core;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.ListView.Strings
{
    public class StringDataDisplay : DataDisplay<StringData>
    {
        [SerializeField] private TextMeshPro label;

        public override void UpdateView()
        {
            base.UpdateView();
            label.text = content.text;
        }
    }
}