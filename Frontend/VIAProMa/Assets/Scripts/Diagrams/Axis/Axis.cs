using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis
{
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

    public List<string> Labels
    {
        get; set;
    }

    public float DataMin { get; set; }

    public float DataMax { get; set; }

    public string Title { get; set; }
}
