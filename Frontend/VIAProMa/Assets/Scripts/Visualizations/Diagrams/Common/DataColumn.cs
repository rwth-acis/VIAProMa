using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class DataColumn<T>
    {
        public List<T> Values { get; private set; }

        public DataColumn()
        {
        }
    }
}