using i5.VIAProMa.DataModel.ReqBaz;
using i5.VIAProMa.UI.ListView.Core;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI.ListView.Categories
{
    public class CategoryDataDisplay : DataDisplay<Category>
    {
        [SerializeField] private TextMeshPro label;

        public override void UpdateView()
        {
            base.UpdateView();
            label.text = content.name;
        }
    }
}