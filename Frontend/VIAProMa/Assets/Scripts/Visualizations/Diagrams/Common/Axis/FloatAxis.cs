using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace i5.ViaProMa.Visualizations.Common
{
    public class FloatAxis : Axis<float>
    {
        protected override float FloatToValue(float f)
        {
            return f;
        }

        protected override float ValueToFloat(float value)
        {
            return value;
        }

        protected override string ValueToString(float value)
        {
            return value.ToString("0.##");
        }
    }
}
