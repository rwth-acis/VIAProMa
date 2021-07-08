using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common.Data.DataSets
{
    public class DataSet
    {
        public List<IDataColumn> DataColumns { get; set; }

        public List<Color> DataPointColors { get; set; }

        public DataSet()
        {
            DataColumns = new List<IDataColumn>();
            DataPointColors = new List<Color>();
        }
    }
}
