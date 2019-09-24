using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
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
