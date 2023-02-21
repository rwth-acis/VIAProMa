using i5.VIAProMa.UI.ListView.Core;
using System;

namespace i5.VIAProMa.DataModel.ReqBaz
{
    [Serializable]
    public class Contributors : IListViewItemData, IUninitializable
    {
        public int id;
        public ReqBazUser creator;
        //public ReqBazUser leadDeveloper;
        public ReqBazUser[] developers;
        public ReqBazUser[] commentCreator;
        public ReqBazUser[] attachmentCreator;

        public bool IsUninitialized
        {
            get
            {
                if (id == 0 && creator.IsUninitialized && developers.Length == 0 && commentCreator.Length == 0 && attachmentCreator.Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}