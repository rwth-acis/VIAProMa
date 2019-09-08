using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public interface IDataColumn
    {
        IAxis GenerateAxis();
    }
}