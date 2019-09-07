using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace i5.ViaProMa.Visualizations.Common
{
    public class FloatAxis : Axis<float>
    {
        public FloatAxis(float dataMin, float dataMax) : base(dataMin, dataMax)
        {
        }

        public FloatAxis(string title, float dataMin, float dataMax) : base(title, dataMin, dataMax)
        {
        }

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
