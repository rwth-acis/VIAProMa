using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data set which consists of data points and the 3 axes
/// </summary>
public class DataSet
{
    public List<DataPoint> Points { get; set; }
    public Axis XAxis { get; set; }
    public Axis YAxis { get; set; }
    public Axis ZAxis { get; set; }

    public DataSet()
    {
        Points = new List<DataPoint>();
        XAxis = new Axis();
        YAxis = new Axis();
        ZAxis = new Axis();
    }
}

/// <summary>
/// The possible types of the diagram axis
/// </summary>
public enum AxisType
{
    NUMERIC, STRING
}
