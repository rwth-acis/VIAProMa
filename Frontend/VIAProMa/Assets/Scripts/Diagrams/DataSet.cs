using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSet
{
    public List<DataPoint> Points { get; set; }
    public Axis XAxis { get; set; }
    public Axis YAxis { get; set; }
    public Axis ZAxis { get; set; }
}

public enum AxisType
{
    NUMERIC, STRING
}
