using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for administering the axis of a diagram
/// </summary>
public class Axis
{
    /// <summary>
    /// The type of the axis, i.e. if it is numeric or has string labels
    /// </summary>
    /// <value></value>
    public AxisType Type
    {
        get
        {
            if (Labels == null)
            {
                return AxisType.NUMERIC;
            }
            else
            {
                return AxisType.STRING;
            }
        }
    }

    /// <summary>
    /// The collection of labels of this axis (null if it is a numeric axis)
    /// </summary>
    /// <value>List of string representing the labels which should be displayed along the axis</value>
    public List<string> Labels
    {
        get; set;
    }

    /// <summary>
    /// The minimum of the numeric data on this aixs
    /// </summary>
    /// <value></value>
    public float DataMin { get; set; }

    /// <summary>
    /// The maximum of the numeric data on this axis
    /// </summary>
    /// <value></value>
    public float DataMax { get; set; }

    /// <summary>
    /// The title of this axis
    /// It will be displayed at the end of the axis as a caption
    /// </summary>
    /// <value></value>
    public string Title { get; set; }
}
