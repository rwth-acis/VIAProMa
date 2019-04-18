using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSet
{
    public List<DataPoint> Points { get; set; }
    public AxisType XAxisType { get; set; }
    public AxisType YAxisType { get; set; }
    public AxisType ZAxisType { get; set; }

    public List<string> XAxisLabels { get; set; }
    public List<string> YAxisLabels { get; set; }
    public List<string> ZAxisLabels { get; set; }
}

public enum AxisType
{
    NUMERIC, STRING
}
