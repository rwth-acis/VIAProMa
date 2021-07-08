using UnityEngine;

namespace i5.VIAProMa.UI.ListView.Core
{
    public class DataDisplay<DataType> : MonoBehaviour, IViewContainer
    where DataType : IListViewItemData
    {
        protected DataType content;

        public DataType Content
        {
            get => content;
        }

        public virtual void Setup(DataType content)
        {
            this.content = content;
            UpdateView();
        }

        public virtual void UpdateView()
        {
        }
    }

    public class DataDisplay : DataDisplay<DataDisplayInspectorData>
    {
    }

    [System.Serializable]
    public class DataDisplayInspectorData : IListViewItemData
    {
    }
}