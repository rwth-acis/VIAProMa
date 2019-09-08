using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatDataConverter : IDataConverter<float>
{
    public float FloatToValue(float f)
    {
        return f;
    }

    public float ValueToFloat(float value)
    {
        return value;
    }

    public string ValueToString(float value)
    {
        return value.ToString("0.##");
    }
}
